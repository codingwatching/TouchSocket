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

using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace TouchSocket.Http.WebSockets;

/// <summary>
/// IWebSocketClosingPlugin
/// </summary>
[DynamicMethod]
public interface IWebSocketClosingPlugin : IPlugin
{
    /// <summary>
    /// 表示收到断开连接报文。如果对方直接断开连接，此方法则不会触发。
    /// </summary>
    /// <param name="webSocket"></param>
    /// <param name="e"></param>
    Task OnWebSocketClosing(IWebSocket webSocket, ClosingEventArgs e);
}