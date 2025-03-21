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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TouchSocket.Http;

/// <summary>
/// 继承自HttpContent的类，用于将Stream数据转换为可发送的HTTP内容。
/// </summary>
public class StreamHttpContent : HttpContent
{
    private readonly int m_bufferLength;

    private readonly HttpFlowOperator m_flowOperator;
    private readonly Stream m_stream;

    /// <summary>
    /// 初始化StreamHttpContent类的新实例。
    /// </summary>
    /// <param name="stream">要包装的流。</param>
    /// <param name="bufferLength">读取数据时使用的缓冲区长度，默认为64KB。</param>
    public StreamHttpContent(Stream stream, int bufferLength = 1024 * 64) : this(stream, new HttpFlowOperator(), bufferLength)
    {
    }

    /// <summary>
    /// 初始化StreamHttpContent类的新实例。
    /// </summary>
    /// <param name="stream">要包装的流。</param>
    /// <param name="flowOperator">用于控制流操作的HttpFlowOperator实例。</param>
    /// <param name="bufferLength">读取数据时使用的缓冲区长度，默认为64KB。</param>
    public StreamHttpContent(Stream stream, HttpFlowOperator flowOperator, int bufferLength = 1024 * 64)
    {
        // 将提供的流分配给内部变量m_stream
        this.m_stream = stream;
        this.m_flowOperator = flowOperator;
        // 将提供的缓冲区长度分配给内部变量m_bufferLength
        this.m_bufferLength = bufferLength;
    }

    /// <inheritdoc/>
    protected override bool OnBuildingContent<TByteBlock>(ref TByteBlock byteBlock)
    {
        return false;
    }

    /// <inheritdoc/>
    protected override void OnBuildingHeader(IHttpHeader header)
    {
        header.Add(HttpHeaders.ContentLength, this.m_stream.Length.ToString());
    }

    /// <inheritdoc/>
    protected override async Task WriteContent(Func<ReadOnlyMemory<byte>, Task> writeFunc, CancellationToken token)
    {
        // 创建一个缓冲区，用于存储读取的数据
        var bytes = BytePool.Default.Rent(this.m_bufferLength);
        var memory = new Memory<byte>(bytes);

        this.m_flowOperator.SetLength(this.GetLength());
        this.m_flowOperator.AddCompletedLength(this.GetPosition());

        try
        {
            while (true)
            {
                var r = await this.m_stream.ReadAsync(memory, token).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                if (r == 0)
                {
                    break;
                }
                await writeFunc.Invoke(memory).ConfigureAwait(EasyTask.ContinueOnCapturedContext);

                await this.m_flowOperator.AddFlowAsync(r).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            }

            this.m_flowOperator.SetResult(Result.Success);
        }
        catch (Exception ex)
        {
            this.m_flowOperator.SetResult(Result.FromException(ex));
            throw;
        }
        finally
        {
            BytePool.Default.Return(bytes);
        }
    }

    private long GetLength()
    {
        try
        {
            return this.m_stream.Length;
        }
        catch
        {
        }

        return 0;
    }

    private long GetPosition()
    {
        try
        {
            return this.m_stream.Position;
        }
        catch
        {
        }

        return 0;
    }
}