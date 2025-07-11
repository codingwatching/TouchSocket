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
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TouchSocket.Sockets;


/// <summary>
/// 表示一个TCP客户端，继承自TcpClientBase并实现了ITcpClient接口。
/// 该类提供了与远程服务器建立TCP连接的功能。
/// </summary>
[System.Diagnostics.DebuggerDisplay("{IP}:{Port}")]
public class TcpClient : TcpClientBase, ITcpClient
{
    #region 事件

    /// <inheritdoc/>
    public ClosedEventHandler<ITcpClient> Closed { get; set; }

    /// <inheritdoc/>
    public ClosingEventHandler<ITcpClient> Closing { get; set; }

    /// <inheritdoc/>
    public ConnectedEventHandler<ITcpClient> Connected { get; set; }

    /// <inheritdoc/>
    public ConnectingEventHandler<ITcpClient> Connecting { get; set; }

    /// <inheritdoc/>
    public ReceivedEventHandler<ITcpClient> Received { get; set; }

    /// <inheritdoc/>
    protected override async Task OnTcpClosed(ClosedEventArgs e)
    {
        if (this.Closed != null)
        {
            await this.Closed.Invoke(this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            if (e.Handled)
            {
                return;
            }
        }

        await base.OnTcpClosed(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <inheritdoc/>
    protected override async Task OnTcpClosing(ClosingEventArgs e)
    {
        if (this.Closing != null)
        {
            await this.Closing.Invoke(this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            if (e.Handled)
            {
                return;
            }
        }

        await base.OnTcpClosing(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <inheritdoc/>
    protected override async Task OnTcpConnected(ConnectedEventArgs e)
    {
        if (this.Connected != null)
        {
            await this.Connected.Invoke(this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            if (e.Handled)
            {
                return;
            }
        }

        await base.OnTcpConnected(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    /// <summary>
    /// 准备连接的时候，此时已初始化Socket，但是并未建立Tcp连接
    /// </summary>
    /// <param name="e"></param>
    protected override async Task OnTcpConnecting(ConnectingEventArgs e)
    {
        if (this.Connecting != null)
        {
            await this.Connecting.Invoke(this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            if (e.Handled)
            {
                return;
            }
        }

        await base.OnTcpConnecting(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    #endregion 事件

    #region Connect

    /// <inheritdoc/>
    public virtual Task ConnectAsync(int millisecondsTimeout, CancellationToken token)
    {
        return this.TcpConnectAsync(millisecondsTimeout, token);
    }

    #endregion Connect

    #region Receiver

    /// <inheritdoc/>
    public void ClearReceiver()
    {
        this.ProtectedClearReceiver();
    }

    /// <inheritdoc/>
    public IReceiver<IReceiverResult> CreateReceiver()
    {
        return this.ProtectedCreateReceiver(this);
    }

    #endregion Receiver

    /// <summary>
    /// 当收到适配器处理的数据时。
    /// </summary>
    /// <param name="e"></param>
    protected override async Task OnTcpReceived(ReceivedDataEventArgs e)
    {
        if (this.Received != null)
        {
            await this.Received.Invoke(this, e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
            if (e.Handled)
            {
                return;
            }
        }

        await base.OnTcpReceived(e).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    #region 异步发送

    /// <inheritdoc/>
    public virtual Task SendAsync(ReadOnlyMemory<byte> memory, CancellationToken token = default)
    {
        return this.ProtectedSendAsync(memory, token);
    }

    /// <inheritdoc/>
    public virtual Task SendAsync(IRequestInfo requestInfo, CancellationToken token = default)
    {
        return this.ProtectedSendAsync(requestInfo, token);
    }

    /// <inheritdoc/>
    [Obsolete("该接口已被弃用，请使用SendAsync直接代替")]
    public virtual Task SendAsync(IList<ArraySegment<byte>> transferBytes)
    {
        return this.ProtectedSendAsync(transferBytes);
    }

    #endregion 异步发送
}