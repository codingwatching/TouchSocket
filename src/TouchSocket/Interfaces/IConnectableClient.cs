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
using System.Threading;
using System.Threading.Tasks;

namespace TouchSocket.Sockets
{

    /// <summary>
    /// 定义可连接客户端的行为。
    /// </summary>
    public interface IConnectableClient
    {
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="millisecondsTimeout">最大等待时间</param>
        /// <param name="token">可取消令箭</param>
        /// <exception cref="TimeoutException">当连接超时时抛出</exception>
        /// <exception cref="Exception">当连接过程中发生错误时抛出</exception>
        Task ConnectAsync(int millisecondsTimeout, CancellationToken token);
    }
}