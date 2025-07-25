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
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TouchSocket.Http;

/// <summary>
/// Http客户端数据处理适配器
/// </summary>
internal sealed class HttpClientDataHandlingAdapter : SingleStreamDataHandlingAdapter
{
    private readonly AsyncAutoResetEvent m_autoResetEvent = new AsyncAutoResetEvent();
    private HttpResponse m_httpResponse;
    private HttpResponse m_httpResponseRoot;
    private long m_surLen;
    private Task m_task;
    private ByteBlock m_tempByteBlock;
    private string s;

    /// <inheritdoc/>
    public override bool CanSplicingSend => false;

    public SingleStreamDataHandlingAdapter WarpAdapter { get; set; }

    /// <inheritdoc/>
    public override void OnLoaded(object owner)
    {
        if (owner is not HttpClientBase clientBase)
        {
            throw new Exception($"此适配器必须适用于{nameof(HttpClientBase)}");
        }
        this.m_httpResponseRoot = new HttpResponse(clientBase);
        base.OnLoaded(owner);
    }

    public void SetCompleteLock()
    {
        this.m_autoResetEvent.Set();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.m_autoResetEvent.SetAll();
            //this.m_autoResetEvent.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <inheritdoc/>
    /// <param name="byteBlock"></param>
    protected override async Task PreviewReceivedAsync(IByteBlockReader byteBlock)
    {
        this.s = byteBlock.ToString();

        if (this.m_tempByteBlock == null)
        {
            byteBlock.Position = 0;
            await this.Single(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        }
        else
        {
            this.m_tempByteBlock.Write(byteBlock.Span);
            var block = this.m_tempByteBlock;
            this.m_tempByteBlock = null;
            block.Position = 0;
            using (block)
            {
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

    private async Task<FilterResult> ReadChunk(IByteBlockReader byteBlock)
    {
        var position = byteBlock.Position;
        var index = byteBlock.Span.Slice(byteBlock.Position, byteBlock.CanReadLength).IndexOf(TouchSocketHttpUtility.CRLF);
        if (index > 0)
        {
            //var headerLength = index - byteBlock.Position;
            var headerLength = index;
            var hex = byteBlock.Span.Slice(byteBlock.Position, headerLength).ToString(Encoding.UTF8);
            var count = hex.ByHexStringToInt32();
            //byteBlock.Position += headerLength + 1;
            byteBlock.Position += headerLength;
            byteBlock.Position += 2;

            if (count > 0)
            {
                if (count > byteBlock.CanReadLength)
                {
                    byteBlock.Position = position;
                    return FilterResult.Cache;
                }

                await this.m_httpResponse.InternalInputAsync(byteBlock.Memory.Slice(byteBlock.Position, count)).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                byteBlock.Position += count;
                byteBlock.Position += 2;
                return FilterResult.GoOn;
            }
            else
            {
                byteBlock.Position += 2;
                return FilterResult.Success;
            }
        }
        else
        {
            return FilterResult.Cache;
        }
    }

    private Task RunGoReceived(HttpResponse response)
    {
        return EasyTask.SafeRun(() => this.GoReceivedAsync(null, response));
    }

    private async Task Single(IByteBlockReader byteBlock)
    {
        while (byteBlock.CanReadLength > 0)
        {
            var adapter = this.WarpAdapter;
            if (adapter != null)
            {
                await adapter.ReceivedInputAsync(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                return;
            }
            if (this.m_httpResponse == null)
            {
                this.m_httpResponseRoot.ResetHttp();
                this.m_httpResponse = this.m_httpResponseRoot;
                if (this.m_httpResponse.ParsingHeader(ref byteBlock))
                {
                    if (this.m_httpResponse.IsChunk || this.m_httpResponse.ContentLength > byteBlock.CanReadLength)
                    {
                        this.m_surLen = this.m_httpResponse.ContentLength;
                        this.m_task = this.RunGoReceived(this.m_httpResponse);
                    }
                    else
                    {
                        this.m_httpResponse.InternalSetContent(byteBlock.ReadToSpan((int)this.m_httpResponse.ContentLength).ToArray());
                        await this.GoReceivedAsync(null, this.m_httpResponse).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        await this.m_autoResetEvent.WaitOneAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        this.m_httpResponse = null;
                    }
                }
                else
                {
                    this.Cache(byteBlock);
                    this.m_httpResponse = null;

                    if (this.m_task != null)
                    {
                        await this.m_task.ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        await this.m_autoResetEvent.WaitOneAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        this.m_task = null;
                    }
                    return;
                }
            }
            else
            {
                if (this.m_httpResponse.IsChunk)
                {
                    switch (await this.ReadChunk(byteBlock).ConfigureAwait(EasyTask.ContinueOnCapturedContext))
                    {
                        case FilterResult.Cache:
                            this.Cache(byteBlock);
                            return;

                        case FilterResult.Success:

                            await this.m_httpResponse.CompleteInput().ConfigureAwait(EasyTask.ContinueOnCapturedContext);

                            this.m_httpResponse = null;
                            if (this.m_task != null)
                            {
                                await this.m_task.ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                                this.m_task = null;
                            }

                            await this.m_autoResetEvent.WaitOneAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                            break;

                        case FilterResult.GoOn:
                        default:
                            break;
                    }
                }
                else if (this.m_surLen > 0)
                {
                    if (byteBlock.CanRead)
                    {
                        var len = (int)Math.Min(this.m_surLen, byteBlock.CanReadLength);
                        await this.m_httpResponse.InternalInputAsync(byteBlock.Memory.Slice(byteBlock.Position, len)).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        this.m_surLen -= len;
                        byteBlock.Position += len;
                        if (this.m_surLen == 0)
                        {
                            await this.m_httpResponse.CompleteInput().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                            this.m_httpResponse = null;
                            if (this.m_task != null)
                            {
                                await this.m_task.ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                                this.m_task = null;
                            }

                            await this.m_autoResetEvent.WaitOneAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        }
                    }
                }
                else
                {
                    this.m_httpResponse = null;
                    if (this.m_task != null)
                    {
                        await this.m_task.ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        this.m_task = null;

                        await this.m_autoResetEvent.WaitOneAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                    }
                }
            }
        }
    }
}