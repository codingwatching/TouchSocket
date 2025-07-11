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
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TouchSocket.Http;

/// <summary>
/// Http服务器数据处理适配器
/// </summary>
public sealed class HttpServerDataHandlingAdapter : SingleStreamDataHandlingAdapter
{
    private HttpRequest m_currentRequest;
    private HttpRequest m_requestRoot;
    private long m_surLen;
    private Task m_task;
    private ByteBlock m_tempByteBlock;

    /// <inheritdoc/>
    public override bool CanSplicingSend => false;

    /// <inheritdoc/>
    public override void OnLoaded(object owner)
    {
        if (owner is not HttpSessionClient httpSessionClient)
        {
            throw new Exception($"此适配器必须适用于{nameof(IHttpService)}");
        }

        this.m_requestRoot = new HttpRequest(httpSessionClient);
        base.OnLoaded(owner);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            //this.m_requestRoot.SafeDispose();
            this.m_tempByteBlock.SafeDispose();
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    /// <param name="byteBlock"></param>
    protected override async Task PreviewReceivedAsync(IByteBlockReader byteBlock)
    {
        if (this.m_tempByteBlock == null)
        {
            byteBlock.Position = 0;
            await this.Single(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        else
        {
            this.m_tempByteBlock.Write(byteBlock.Span);
            using (var block = this.m_tempByteBlock)
            {
                this.m_tempByteBlock = null;
                block.Position = 0;
                await this.Single(block).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            }
        }
    }

    private void Cache(IByteBlockReader byteBlock)
    {
        if (byteBlock.CanReadLength > 0)
        {
            this.m_tempByteBlock = new ByteBlock(1024 * 64);
            this.m_tempByteBlock.Write(byteBlock.Span.Slice(byteBlock.Position, byteBlock.CanReadLength));
            if (this.m_tempByteBlock.Length > this.MaxPackageSize)
            {
                this.OnError(default, "缓存的数据长度大于设定值的情况下未收到解析信号", true, true);
            }
        }
    }

    //private void DestroyRequest()
    //{
    //    this.m_currentRequest = null;
    //    //if (this.m_task != null)
    //    //{
    //    //    await this.m_task.ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    //    //    this.m_task = null;
    //    //}
    //}

    private async Task Single(IByteBlockReader byteBlock)
    {
        while (byteBlock.CanReadLength > 0)
        {
            if (this.DisposedValue)
            {
                return;
            }
            if (this.m_currentRequest == null)
            {
                if (this.m_task != null)
                {
                    await this.m_task.ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                    this.m_task = null;
                }

                this.m_requestRoot.ResetHttp();
                this.m_currentRequest = this.m_requestRoot;
                if (this.m_currentRequest.ParsingHeader(ref byteBlock))
                {
                    //byteBlock.Position++;
                    if (this.m_currentRequest.ContentLength > byteBlock.CanReadLength)
                    {
                        this.m_surLen = this.m_currentRequest.ContentLength;

                        this.m_task = this.TaskRunGoReceived(this.m_currentRequest);
                    }
                    else
                    {
                        var contentLength = (int)this.m_currentRequest.ContentLength;
                        var content = byteBlock.Memory.Slice(byteBlock.Position, contentLength);
                        byteBlock.Position += contentLength;

                        this.m_currentRequest.InternalSetContent(content);

                        this.m_task = this.TaskRunGoReceived(this.m_currentRequest);

                        this.m_currentRequest = null;
                    }
                }
                else
                {
                    this.Cache(byteBlock);
                    this.m_currentRequest = null;
                    return;
                }
            }

            if (this.m_surLen > 0)
            {
                if (byteBlock.CanRead)
                {
                    var len = (int)Math.Min(this.m_surLen, byteBlock.CanReadLength);

                    await this.m_currentRequest.InternalInputAsync(byteBlock.Memory.Slice(byteBlock.Position, len)).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                    this.m_surLen -= len;
                    byteBlock.Position += len;
                    if (this.m_surLen == 0)
                    {
                        await this.m_currentRequest.CompleteInput().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        this.m_currentRequest = null;
                    }
                }
            }
            else
            {
                this.m_currentRequest = null;
            }
        }
    }

    private Task TaskRunGoReceived(HttpRequest request)
    {
        var task = EasyTask.SafeRun<ByteBlock, IRequestInfo>(this.GoReceivedAsync, null, request);
        return task;
    }
}