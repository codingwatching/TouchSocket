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

namespace TouchSocket.Sockets;

internal sealed class TcpReconnectionPlugin<TClient> : ReconnectionPlugin<TClient>, ITcpClosedPlugin where TClient : ITcpClient
{
    public override Func<TClient, int, Task<bool?>> ActionForCheck { get; set; }

    public TcpReconnectionPlugin()
    {
        this.ActionForCheck = (c, i) => Task.FromResult<bool?>(c.Online);
    }

    public async Task OnTcpClosed(ITcpSession client, ClosedEventArgs e)
    {
        await e.InvokeNext().ConfigureAwait(EasyTask.ContinueOnCapturedContext);

        if (client is not TClient tClient)
        {
            return;
        }

        if (e.Manual)
        {
            return;
        }

        _ = EasyTask.SafeRun(async () =>
        {
            while (true)
            {
                if (this.DisposedValue)
                {
                    return;
                }
                if (await this.ActionForConnect.Invoke(tClient).ConfigureAwait(EasyTask.ContinueOnCapturedContext))
                {
                    return;
                }
            }
        });
    }
}