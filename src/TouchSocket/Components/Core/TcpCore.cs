//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://touchsocket.net/
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Resources;

namespace TouchSocket.Sockets;

/// <summary>
/// Tcp核心
/// </summary>
internal sealed class TcpCore : DisposableObject
{
    /// <summary>
    /// Tcp核心
    /// </summary>
    public TcpCore()
    {
        this.m_receiveCounter = new ValueCounter
        {
            Period = TimeSpan.FromSeconds(1),
            OnPeriod = this.OnReceivePeriod
        };

        this.m_sentCounter = new ValueCounter
        {
            Period = TimeSpan.FromSeconds(1),
            OnPeriod = this.OnSendPeriod
        };

        _ = EasyTask.SafeRun(this.TaskSend, this.m_cancellationTokenSource.Token);
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~TcpCore()
    {
        this.Dispose(disposing: false);
    }

    /// <summary>
    /// 最大缓存尺寸
    /// </summary>
    public int MaxBufferSize { get; set; } = 1024 * 512;

    /// <summary>
    /// 最小缓存尺寸
    /// </summary>
    public int MinBufferSize { get; set; } = 1024 * 10;

    #region 字段

    private const int BatchSize = 50;
    private const int MaxMemoryLength = 1024;
    private readonly ConcurrentQueue<SendSegment> m_asyncQueueForSend = new();
    private readonly AsyncManualResetEvent m_asyncResetEventForSend = new(true);
    private readonly AsyncAutoResetEvent m_asyncResetEventForTask = new();
    private readonly CancellationTokenSource m_cancellationTokenSource = new();
    private readonly SemaphoreSlim m_semaphoreForSend = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim m_semaphoreSlimForMax = new SemaphoreSlim(BatchSize, BatchSize);
    private readonly SocketReceiver m_socketReceiver = new SocketReceiver();
    private readonly SocketSender m_socketSender = new SocketSender();
    private ExceptionDispatchInfo m_exceptionDispatchInfo;
    private bool m_noDelay;
    private int m_receiveBufferSize = 1024 * 10;
    private ValueCounter m_receiveCounter;
    private int m_sendBufferSize = 1024 * 10;
    private ValueCounter m_sentCounter;
    private Socket m_socket;
    private SslStream m_sslStream;
    private bool m_useSsl;
    private short m_version;

    #endregion 字段

    public bool NoDelay => this.m_noDelay;

    /// <summary>
    /// 接收缓存池,运行时的值会根据流速自动调整
    /// </summary>
    public int ReceiveBufferSize => Math.Min(Math.Max(this.m_receiveBufferSize, this.MinBufferSize), this.MaxBufferSize);

    /// <summary>
    /// 接收计数器
    /// </summary>
    public ValueCounter ReceiveCounter => this.m_receiveCounter;

    public SemaphoreSlim SemaphoreForSend => this.m_semaphoreForSend;

    /// <summary>
    /// 发送缓存池,运行时的值会根据流速自动调整
    /// </summary>
    public int SendBufferSize => Math.Min(Math.Max(this.m_sendBufferSize, this.MinBufferSize), this.MaxBufferSize);

    /// <summary>
    /// 发送计数器
    /// </summary>
    public ValueCounter SendCounter => this.m_sentCounter;

    /// <summary>
    /// Socket
    /// </summary>
    public Socket Socket => this.m_socket;

    /// <summary>
    /// 提供一个用于客户端-服务器通信的流，该流使用安全套接字层 (SSL) 安全协议对服务器和（可选）客户端进行身份验证。
    /// </summary>
    public SslStream SslStream => this.m_sslStream;

    /// <summary>
    /// 是否启用了Ssl
    /// </summary>
    public bool UseSsl => this.m_useSsl;

    /// <summary>
    /// 以Ssl服务器模式授权
    /// </summary>
    /// <param name="sslOption"></param>
    /// <returns></returns>
    public async Task AuthenticateAsync(ServiceSslOption sslOption)
    {
        if (this.m_useSsl)
        {
            return;
        }
        var sslStream = (sslOption.CertificateValidationCallback != null) ? new SslStream(new NetworkStream(this.m_socket, false), false, sslOption.CertificateValidationCallback) : new SslStream(new NetworkStream(this.m_socket, false), false);

        await sslStream.AuthenticateAsServerAsync(sslOption.Certificate, sslOption.ClientCertificateRequired
            , sslOption.SslProtocols
            , sslOption.CheckCertificateRevocation).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

        //await sslStream.AuthenticateAsServerAsync(sslOption.Certificate).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

        this.m_sslStream = sslStream;
        this.m_useSsl = true;
    }

    /// <summary>
    /// 以Ssl客户端模式授权
    /// </summary>
    /// <param name="sslOption"></param>
    /// <returns></returns>
    public async Task AuthenticateAsync(ClientSslOption sslOption)
    {
        if (this.m_useSsl)
        {
            return;
        }

        var sslStream = (sslOption.CertificateValidationCallback != null) ? new SslStream(new NetworkStream(this.m_socket, false), false, sslOption.CertificateValidationCallback) : new SslStream(new NetworkStream(this.m_socket, false), false);
        if (sslOption.ClientCertificates == null)
        {
            await sslStream.AuthenticateAsClientAsync(sslOption.TargetHost).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        else
        {
            await sslStream.AuthenticateAsClientAsync(sslOption.TargetHost, sslOption.ClientCertificates, sslOption.SslProtocols, sslOption.CheckCertificateRevocation).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        this.m_sslStream = sslStream;
        this.m_useSsl = true;
    }

    #region Receive
    public ValueTask<TcpOperationResult> ReceiveAsync(in Memory<byte> memory, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (this.m_useSsl)
        {
            return this.SslReceiveAsync(memory, token);
        }
        return this.m_socketReceiver.ReceiveAsync(this.m_socket, memory);
    }

    public async ValueTask<TcpOperationResult> SslReceiveAsync(Memory<byte> memory, CancellationToken token)
    {
        var r = await this.m_sslStream.ReadAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        this.m_receiveCounter.Increment(r);

        return new TcpOperationResult(r, SocketError.Success);
    }

    public bool ReceiveIncrement(long value)
    {
        return this.m_receiveCounter.Increment(value);
    }
    #endregion


    /// <summary>
    /// 重置环境，并设置新的<see cref="Socket"/>。
    /// </summary>
    /// <param name="socket"></param>
    public void Reset(Socket socket)
    {
        ThrowHelper.ThrowArgumentNullExceptionIf(socket, nameof(socket));

        if (!socket.Connected)
        {
            ThrowHelper.ThrowException(TouchSocketResource.SocketHaveToConnected);
        }
        this.Reset();
        this.m_socket = socket;
        this.m_noDelay = socket.NoDelay;
    }

    /// <summary>
    /// 重置环境。
    /// </summary>
    public void Reset()
    {
        this.m_version++;
        this.m_exceptionDispatchInfo = null;
        this.m_useSsl = false;
        this.m_receiveCounter.Reset();
        this.m_sentCounter.Reset();
        this.m_sslStream?.Dispose();
        this.m_sslStream = null;
        this.m_socket = null;
        this.m_receiveBufferSize = this.MinBufferSize;
        this.m_sendBufferSize = this.MinBufferSize;
    }

    public async Task<Result> ShutdownAsync(SocketShutdown how)
    {
        //主要等待发送队列任务完成
        await this.m_semaphoreForSend.WaitAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        try
        {
            await this.m_asyncResetEventForSend.WaitOneAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            this.m_socket.Shutdown(how);

            return Result.Success;
        }
        catch (Exception ex)
        {
            return Result.FromException(ex);
        }
        finally
        {
            this.m_semaphoreForSend.Release();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            this.m_socketReceiver.SafeDispose();
            this.m_socketSender.SafeDispose();

            this.m_semaphoreForSend.SafeDispose();
            this.m_semaphoreSlimForMax.SafeDispose();

            this.m_asyncResetEventForSend.Set();
            this.m_asyncResetEventForTask.SetAll();

            this.m_cancellationTokenSource.SafeCancel();
            this.m_cancellationTokenSource.SafeDispose();
        }
    }

    #region Send

    /// <summary>
    /// 异步发送数据。
    /// <para>
    /// 内部会根据是否启用Ssl，进行直接发送，还是Ssl发送。
    /// </para>
    /// </summary>
    /// <param name="memory">数据</param>
    /// <param name="token">可取消令箭</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task SendAsync(ReadOnlyMemory<byte> memory, CancellationToken token)
    {
        await this.m_semaphoreForSend.WaitAsync(token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        try
        {
            await this.UnsafeSendAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        finally
        {
            this.m_semaphoreForSend.Release();
        }
    }

    public async Task UnsafeSendAsync(ReadOnlyMemory<byte> memory, CancellationToken token)
    {
        if (this.NoDelay)
        {
            await this.PrivateSendAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            return;
        }
        if (memory.Length < MaxMemoryLength)
        {
            var dispatchInfo = this.m_exceptionDispatchInfo;
            dispatchInfo?.Throw();

            await this.m_semaphoreSlimForMax.WaitAsync(token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

            var sendSegment = ArrayPool<byte>.Shared.Rent(memory.Length);

            memory.CopyTo(new Memory<byte>(sendSegment, 0, memory.Length));

            this.m_asyncQueueForSend.Enqueue(new SendSegment(sendSegment, memory.Length, this.m_version));
            this.m_asyncResetEventForSend.Reset();

            this.m_asyncResetEventForTask.Set();
            return;
        }

        await this.m_asyncResetEventForSend.WaitOneAsync(token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        await this.PrivateSendAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    public Task UnsafeSendAsync(in ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        return this.PrivateSendAsync(buffer, token);
    }

    private async Task PrivateSendAsync(ReadOnlyMemory<byte> memory, CancellationToken token)
    {
        var length = memory.Length;
        if (this.m_useSsl)
        {
            await this.SslStream.WriteAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        else
        {
            var offset = 0;
            while (length > 0)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    var result = await this.m_socketSender.SendAsync(this.m_socket, memory).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                    if (result.SocketError != SocketError.Success)
                    {
                        throw new SocketException((int)result.SocketError);
                    }
                    if (result.BytesTransferred == 0 && length > 0)
                    {
                        ThrowHelper.ThrowException(TouchSocketResource.IncompleteDataTransmission);
                    }
                    offset += result.BytesTransferred;
                    length -= result.BytesTransferred;
                }
                finally
                {
                    this.m_socketSender.Reset();
                }
            }
        }
        this.m_sentCounter.Increment(length);
    }

    private async Task PrivateSendAsync(List<ArraySegment<byte>> buffer, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var length = 0;
        if (this.m_useSsl)
        {
            foreach (var memory in buffer)
            {
                await this.SslStream.WriteAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                length += memory.Count;
            }
        }
        else
        {
            try
            {
                var result = await this.m_socketSender.SendAsync(this.m_socket, buffer).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                if (result.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)result.SocketError);
                }

                foreach (var memory in buffer)
                {
                    length += memory.Count;
                }

                if (result.BytesTransferred != length)
                {
                    ThrowHelper.ThrowException(TouchSocketResource.IncompleteDataTransmission);
                }
            }
            finally
            {
                this.m_socketSender.Reset();
            }
        }
        this.m_sentCounter.Increment(length);
    }

    private async Task PrivateSendAsync(ReadOnlySequence<byte> buffer, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var length = 0;
        if (this.m_useSsl)
        {
            foreach (var memory in buffer)
            {
                await this.SslStream.WriteAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                length += memory.Length;
            }
        }
        else
        {
            try
            {
                var result = await this.m_socketSender.SendAsync(this.m_socket, buffer).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                if (result.SocketError != SocketError.Success)
                {
                    throw new SocketException((int)result.SocketError);
                }

                foreach (var memory in buffer)
                {
                    length += memory.Length;
                }

                if (result.BytesTransferred != length)
                {
                    ThrowHelper.ThrowException(TouchSocketResource.IncompleteDataTransmission);
                }
            }
            finally
            {
                this.m_socketSender.Reset();
            }
        }
        this.m_sentCounter.Increment(length);
    }

    private async Task TaskSend(CancellationToken token)
    {
        //var valuesToProcess = new SendSegment[BatchSize];
        var buffer = new List<ArraySegment<byte>>(BatchSize);
        while (!token.IsCancellationRequested)
        {
            // 尝试填充数组
            while (buffer.Count < BatchSize && this.m_asyncQueueForSend.TryDequeue(out var value))
            {
                if (value.version == this.m_version)
                {
                    buffer.Add(new ArraySegment<byte>(value.Data, 0, value.Length));
                }
                this.m_semaphoreSlimForMax.Release();
            }

            if (token.IsCancellationRequested)
            {
                return;
            }

            // 如果有元素需要处理，并且没有异常
            if (buffer.Count > 0 && this.m_exceptionDispatchInfo == null)
            {
                try
                {
                    await this.PrivateSendAsync(buffer, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                }
                catch (Exception ex)
                {
                    this.m_exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    this.m_asyncQueueForSend.Clear();
                }
                finally
                {
                    foreach (var item in buffer)
                    {
                        ArrayPool<byte>.Shared.Return(item.Array);
                    }
                    buffer.Clear();
                }
            }
            else
            {
                //Debug.WriteLine("Pause");
                // 队列为空，设置事件并等待
                this.m_asyncResetEventForSend.Set();

                await this.m_asyncResetEventForTask.WaitOneAsync(token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            }
        }
    }

    #endregion Send

    private void OnReceivePeriod(long value)
    {
        this.m_receiveBufferSize = Math.Max(TouchSocketCoreUtility.HitBufferLength(value), this.MinBufferSize);
    }

    private void OnSendPeriod(long value)
    {
        this.m_sendBufferSize = Math.Max(TouchSocketCoreUtility.HitBufferLength(value), this.MinBufferSize);
    }

    #region Class

    private struct SendSegment
    {
        public byte[] Data;
        public int Length;
        public short version;

        public SendSegment(byte[] bytes, int length, short version)
        {
            this.Data = bytes;
            this.Length = length;
            this.version = version;
        }
    }

    #endregion Class
}