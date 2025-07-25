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
using System.Collections.Generic;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Resources;
using TouchSocket.Sockets;

namespace TouchSocket.NamedPipe;

/// <summary>
/// 命名管道客户端客户端基类
/// </summary>
public abstract class NamedPipeClientBase : SetupConfigObject, INamedPipeSession
{
    /// <summary>
    /// 命名管道客户端客户端基类
    /// </summary>
    public NamedPipeClientBase()
    {
        this.Protocol = Protocol.NamedPipe;
        this.m_receiveCounter = new ValueCounter
        {
            Period = TimeSpan.FromSeconds(1),
            OnPeriod = this.OnPeriod
        };
    }

    #region 变量

    private readonly SemaphoreSlim m_semaphoreSlimForConnect = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim m_semaphoreSlimForSend = new SemaphoreSlim(1, 1);
    private volatile bool m_online;
    private Task m_receiveTask;
    private NamedPipeClientStream m_pipeStream;
    private int m_receiveBufferSize = 1024 * 10;
    private ValueCounter m_receiveCounter;
    private InternalReceiver m_receiver;
    private SingleStreamDataHandlingAdapter m_dataHandlingAdapter;
    private readonly Lock m_lockForAbort = new Lock();
    private CancellationTokenSource m_tokenSourceForOnline;
    #endregion 变量

    #region 事件

    /// <summary>
    /// 已经建立管道连接
    /// </summary>
    /// <param name="e"></param>
    protected virtual async Task OnNamedPipeConnected(ConnectedEventArgs e)
    {
        await this.PluginManager.RaiseAsync(typeof(INamedPipeConnectedPlugin), this.Resolver, this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <summary>
    /// 准备连接的时候
    /// </summary>
    /// <param name="e"></param>
    protected virtual async Task OnNamedPipeConnecting(ConnectingEventArgs e)
    {
        await this.PluginManager.RaiseAsync(typeof(INamedPipeConnectingPlugin), this.Resolver, this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <summary>
    /// 断开连接。在客户端未设置连接状态时，不会触发
    /// </summary>
    /// <param name="e"></param>
    protected virtual async Task OnNamedPipeClosed(ClosedEventArgs e)
    {
        await this.PluginManager.RaiseAsync(typeof(INamedPipeClosedPlugin), this.Resolver, this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <summary>
    /// 即将断开连接(仅主动断开时有效)。
    /// </summary>
    /// <param name="e"></param>
    protected virtual async Task OnNamedPipeClosing(ClosingEventArgs e)
    {
        await this.PluginManager.RaiseAsync(typeof(INamedPipeClosingPlugin), this.Resolver, this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    private async Task PrivateOnNamedPipeConnected(ConnectedEventArgs e)
    {
        await this.OnNamedPipeConnected(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    private async Task PrivateOnNamedPipeConnecting(ConnectingEventArgs e)
    {
        await this.OnNamedPipeConnecting(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        if (this.m_dataHandlingAdapter == null)
        {
            var adapter = this.Config.GetValue(NamedPipeConfigExtension.NamedPipeDataHandlingAdapterProperty)?.Invoke();
            if (adapter != null)
            {
                this.SetAdapter(adapter);
            }
        }
    }

    private async Task PrivateOnNamedPipeClosed((ClosedEventArgs e, Task receiveTask, InternalReceiver receiver) value)
    {
        await value.receiveTask.SafeWaitAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        if (value.receiver != null)
        {
            await value.receiver.Complete(value.e.Message).SafeWaitAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }

        await this.OnNamedPipeClosed(value.e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    private async Task PrivateOnNamedPipeClosing(ClosingEventArgs e)
    {
        await this.OnNamedPipeClosing(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    #endregion 事件

    #region 属性

    /// <inheritdoc/>
    public bool IsClient => true;

    /// <inheritdoc/>
    public DateTimeOffset LastReceivedTime { get; private set; }

    /// <inheritdoc/>
    public DateTimeOffset LastSentTime { get; private set; }

    /// <inheritdoc/>
    public PipeStream PipeStream => this.m_pipeStream;

    /// <inheritdoc/>
    public Protocol Protocol { get; protected set; }

    /// <inheritdoc/>
    public SingleStreamDataHandlingAdapter DataHandlingAdapter => this.m_dataHandlingAdapter;

    /// <inheritdoc/>
    public virtual bool Online => this.m_online;

    /// <inheritdoc/>
    public CancellationToken  ClosedToken => m_tokenSourceForOnline.GetTokenOrCanceled();

    #endregion 属性

    #region 断开操作

    /// <summary>
    /// 中断链接
    /// </summary>
    /// <param name="manual"></param>
    /// <param name="msg"></param>
    protected void Abort(bool manual, string msg)
    {
        lock (this.m_lockForAbort)
        {
            if (this.m_online)
            {
                this.m_online = false;
                this.m_pipeStream.SafeDispose();

                this.m_dataHandlingAdapter.SafeDispose();
                this.m_dataHandlingAdapter = default;
                this.m_tokenSourceForOnline.SafeCancel();
                this.m_tokenSourceForOnline.SafeDispose();
                _ = EasyTask.SafeRun(this.PrivateOnNamedPipeClosed, (new ClosedEventArgs(manual, msg), this.m_receiveTask, this.m_receiver));
            }
        }
    }


    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (this.DisposedValue)
        {
            return;
        }

        if (disposing)
        {
            this.Abort(true, TouchSocketResource.DisposeClose);
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public virtual async Task<Result> CloseAsync(string msg, CancellationToken token = default)
    {
        try
        {
            if (this.m_online)
            {
                await this.PrivateOnNamedPipeClosing(new ClosedEventArgs(true, msg)).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

                lock (this.m_lockForAbort)
                {
                    this.m_pipeStream.Close();
                    this.Abort(true, msg);
                }
            }
            return Result.Success;
        }
        catch (Exception ex)
        {
            return Result.FromException(ex);
        }
    }

    #endregion 断开操作

    #region Connect

    /// <summary>
    /// 建立管道的连接。
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    /// <param name="token">可取消令箭</param>
    /// <exception cref="ObjectDisposedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    /// <exception cref="TimeoutException"></exception>
    protected async Task PipeConnectAsync(int millisecondsTimeout, CancellationToken token)
    {
        await this.m_semaphoreSlimForConnect.WaitTimeAsync(millisecondsTimeout, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        try
        {
            if (this.m_online)
            {
                return;
            }
            this.ThrowIfDisposed();
            this.ThrowIfConfigIsNull();

            var pipeName = this.Config.GetValue(NamedPipeConfigExtension.PipeNameProperty);
            ThrowHelper.ThrowArgumentNullExceptionIf(pipeName, nameof(pipeName));

            var serverName = this.Config.GetValue(NamedPipeConfigExtension.PipeServerNameProperty);
            this.m_pipeStream.SafeDispose();

            var namedPipe = CreatePipeClient(serverName, pipeName);
            await this.PrivateOnNamedPipeConnecting(new ConnectingEventArgs()).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

            await namedPipe.ConnectAsync(millisecondsTimeout, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

            if (!namedPipe.IsConnected)
            {
                ThrowHelper.ThrowException(TouchSocketCoreResource.UnknownError);
            }

            this.m_tokenSourceForOnline = new CancellationTokenSource();
            this.m_pipeStream = namedPipe;
            this.m_online = true;
            this.m_receiveTask = EasyTask.SafeRun(this.BeginReceive, this.m_tokenSourceForOnline.Token);
            _ = EasyTask.SafeRun(this.PrivateOnNamedPipeConnected, new ConnectedEventArgs());
        }
        finally
        {
            this.m_semaphoreSlimForConnect.Release();
        }
    }

    #endregion Connect

    #region Receiver

    /// <inheritdoc/>
    protected void ProtectedClearReceiver()
    {
        this.m_receiver = null;
    }

    /// <inheritdoc/>
    protected IReceiver<IReceiverResult> ProtectedCreateReceiver(IReceiverClient<IReceiverResult> receiverObject)
    {
        return this.m_receiver ??= new InternalReceiver(receiverObject);
    }

    #endregion Receiver

    /// <summary>
    /// 处理已接收到的数据。
    /// <para>根据不同的数据处理适配器，会传递不同的数据</para>
    /// </summary>
    /// <param name="e"></param>
    /// <returns>如果返回<see langword="true"/>则表示数据已被处理，且不会再向下传递。</returns>
    protected virtual async Task OnNamedPipeReceived(ReceivedDataEventArgs e)
    {
        await this.PluginManager.RaiseAsync(typeof(INamedPipeReceivedPlugin), this.Resolver, this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <summary>
    /// 当收到原始数据
    /// </summary>
    /// <param name="byteBlock"></param>
    /// <returns>如果返回<see langword="true"/>则表示数据已被处理，且不会再向下传递。</returns>
    protected virtual ValueTask<bool> OnNamedPipeReceiving(ByteBlock byteBlock)
    {
        return this.PluginManager.RaiseAsync(typeof(INamedPipeReceivingPlugin), this.Resolver, this, new ByteBlockEventArgs(byteBlock));
    }


    /// <summary>
    /// 触发命名管道发送事件的异步方法。
    /// </summary>
    /// <param name="memory">待发送的字节内存。</param>
    /// <returns>一个等待任务，结果指示发送操作是否成功。</returns>
    protected virtual ValueTask<bool> OnNamedPipeSending(ReadOnlyMemory<byte> memory)
    {
        // 将发送任务委托给插件管理器，以便在所有相关的插件中引发命名管道发送事件
        return this.PluginManager.RaiseAsync(typeof(INamedPipeSendingPlugin), this.Resolver, this, new SendingEventArgs(memory));
    }

    /// <summary>
    /// 设置适配器
    /// </summary>
    /// <param name="adapter">要设置的适配器实例</param>
    protected void SetAdapter(SingleStreamDataHandlingAdapter adapter)
    {
        // 检查当前实例是否已被释放
        this.ThrowIfDisposed();

        ThrowHelper.ThrowArgumentNullExceptionIf(adapter, nameof(adapter));

        // 如果当前实例已有配置，则将配置应用到新适配器上
        if (this.Config != null)
        {
            adapter.Config(this.Config);
        }

        // 将当前实例的日志记录器设置到适配器上
        adapter.Logger = this.Logger;
        // 调用适配器的OnLoaded方法，通知适配器已被加载
        adapter.OnLoaded(this);
        // 设置适配器接收数据时的回调方法
        adapter.ReceivedAsyncCallBack = this.PrivateHandleReceivedData;
        // 设置适配器发送数据时的异步回调方法
        adapter.SendAsyncCallBack = this.ProtectedDefaultSendAsync;
        // 将适配器实例设置为当前数据处理适配器
        this.m_dataHandlingAdapter = adapter;
    }

    private static NamedPipeClientStream CreatePipeClient(string serverName, string pipeName)
    {
        var pipeClient = new NamedPipeClientStream(serverName, pipeName, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
        return pipeClient;
    }

    private async Task BeginReceive(CancellationToken token)
    {
        while (true)
        {
            using (var byteBlock = new ByteBlock(this.GetReceiveBufferSize()))
            {
                try
                {
                    var r = await this.m_pipeStream.ReadAsync(byteBlock.TotalMemory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                    if (r == 0)
                    {
                        this.Abort(false, "远程终端主动关闭");
                        return;
                    }

                    byteBlock.SetLength(r);
                    await this.HandleReceivingData(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                }
                catch (Exception ex)
                {
                    this.Abort(false, ex.Message);
                    return;
                }
            }
        }
    }

    private int GetReceiveBufferSize()
    {
        var minBufferSize = this.Config.GetValue(TouchSocketConfigExtension.MinBufferSizeProperty) ?? 1024 * 10;
        var maxBufferSize = this.Config.GetValue(TouchSocketConfigExtension.MaxBufferSizeProperty) ?? 1024 * 512;
        return Math.Min(Math.Max(this.m_receiveBufferSize, minBufferSize), maxBufferSize);
    }

    private async Task HandleReceivingData(ByteBlock byteBlock)
    {
        try
        {
            if (this.DisposedValue)
            {
                return;
            }

            this.m_receiveCounter.Increment(byteBlock.Length);

            if (await this.OnNamedPipeReceiving(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext))
            {
                return;
            }

            if (this.m_dataHandlingAdapter == null)
            {
                await this.PrivateHandleReceivedData(byteBlock, default).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            }
            else
            {
                await this.m_dataHandlingAdapter.ReceivedInputAsync(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            }
        }
        catch (Exception ex)
        {
            this.Logger?.Log(LogLevel.Error, this, "在处理数据时发生错误", ex);
        }
    }

    private void OnPeriod(long value)
    {
        this.m_receiveBufferSize = TouchSocketCoreUtility.HitBufferLength(value);
    }

    private async Task PrivateHandleReceivedData(IByteBlockReader byteBlock, IRequestInfo requestInfo)
    {
        var receiver = this.m_receiver;
        if (receiver != null)
        {
            await receiver.InputReceiveAsync(byteBlock, requestInfo).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            return;
        }
        await this.OnNamedPipeReceived(new ReceivedDataEventArgs(byteBlock, requestInfo)).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    #region Throw

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfCannotSendRequestInfo()
    {
        if (this.m_dataHandlingAdapter == null || !this.m_dataHandlingAdapter.CanSendRequestInfo)
        {
            throw new NotSupportedException($"当前适配器为空或者不支持对象发送。");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfClientNotConnected()
    {
        if (this.m_online)
        {
            return;
        }

        ThrowHelper.ThrowClientNotConnectedException();
    }

    #endregion Throw

    #region 直接发送
    /// <inheritdoc/>
    protected async Task ProtectedDefaultSendAsync(ReadOnlyMemory<byte> memory, CancellationToken token)
    {
        this.ThrowIfDisposed();
        this.ThrowIfClientNotConnected();
        await this.OnNamedPipeSending(memory).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        await this.m_semaphoreSlimForSend.WaitAsync(token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        try
        {
            await this.m_pipeStream.WriteAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            this.LastSentTime = DateTimeOffset.UtcNow;
        }
        finally
        {
            this.m_semaphoreSlimForSend.Release();
        }
    }

    #endregion 直接发送

    #region 异步发送

    /// <summary>
    /// 异步发送数据，根据是否配置了数据处理适配器来决定数据的发送方式。
    /// </summary>
    /// <param name="memory">待发送的字节内存。</param>
    /// <param name="token">可取消令箭</param>
    /// <returns>一个异步任务，表示发送操作。</returns>
    protected Task ProtectedSendAsync(in ReadOnlyMemory<byte> memory, CancellationToken token)
    {
        // 如果未配置数据处理适配器，则使用默认的发送方式。
        if (this.m_dataHandlingAdapter == null)
        {
            return this.ProtectedDefaultSendAsync(memory,token);
        }
        else
        {
            // 如果配置了数据处理适配器，则使用适配器指定的发送方式。
            return this.m_dataHandlingAdapter.SendInputAsync(memory,token);
        }
    }


    /// <summary>
    /// 异步安全发送请求信息。
    /// </summary>
    /// <param name="requestInfo">请求信息对象，包含要发送的数据。</param>
    /// <param name="token">可取消令箭</param>
    /// <returns>返回一个任务，表示异步操作的结果。</returns>
    /// <remarks>
    /// 此方法用于在发送请求之前验证是否可以发送请求信息，
    /// 并通过<see cref="DataHandlingAdapter"/>适配器安全处理发送过程。
    /// </remarks>
    protected Task ProtectedSendAsync(IRequestInfo requestInfo, CancellationToken token)
    {
        // 验证当前状态是否允许发送请求信息，如果不允许则抛出异常。
        this.ThrowIfCannotSendRequestInfo();
        // 调用ProtectedDataHandlingAdapter的SendInputAsync方法异步发送请求信息。
        return this.m_dataHandlingAdapter.SendInputAsync(requestInfo, token);
    }

    /// <summary>
    /// 异步发送经过处理的数据。
    /// 如果ProtectedDataHandlingAdapter未设置或者不支持拼接发送，则将transferBytes合并到一个连续的内存块中再发送。
    /// 如果ProtectedDataHandlingAdapter已设置且支持拼接发送，则直接发送transferBytes。
    /// </summary>
    /// <param name="transferBytes">待发送的字节数据列表，每个元素包含要传输的字节片段。</param>
    /// <returns>发送任务。</returns>
    [Obsolete("该接口已被弃用，请使用SendAsync直接代替")]
    protected async Task ProtectedSendAsync(IList<ArraySegment<byte>> transferBytes)
    {
        // 检查ProtectedDataHandlingAdapter是否已设置且支持拼接发送
        if (this.m_dataHandlingAdapter == null || !this.m_dataHandlingAdapter.CanSplicingSend)
        {
            // 如果不支持拼接发送，计算所有字节片段的总长度
            var length = 0;
            foreach (var item in transferBytes)
            {
                length += item.Count;
            }
            // 使用计算出的总长度创建一个连续的内存块
            using (var byteBlock = new ByteBlock(length))
            {
                // 将每个字节片段写入连续的内存块
                foreach (var item in transferBytes)
                {
                    byteBlock.Write(new ReadOnlySpan<byte>(item.Array, item.Offset, item.Count));
                }
                // 根据ProtectedDataHandlingAdapter的状态选择发送方法
                if (this.m_dataHandlingAdapter == null)
                {
                    // 如果未设置ProtectedDataHandlingAdapter，使用默认发送方法
                    await this.ProtectedDefaultSendAsync(byteBlock.Memory,CancellationToken.None).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                }
                else
                {
                    // 如果已设置ProtectedDataHandlingAdapter但不支持拼接发送，使用Adapter的发送方法
                    await this.m_dataHandlingAdapter.SendInputAsync(byteBlock.Memory,CancellationToken.None).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                }
            }
        }
        else
        {
            // 如果已设置ProtectedDataHandlingAdapter且支持拼接发送，直接使用Adapter的发送方法
            await this.m_dataHandlingAdapter.SendInputAsync(transferBytes).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
    }

    #endregion 异步发送
}