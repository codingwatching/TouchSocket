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

using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;

namespace TouchSocket.Sockets;

/// <summary>  
/// 具有关闭动作的对象。  
/// </summary>  
public interface IClosableClient
{
    /// <summary>  
    /// 获取一个 <see cref="CancellationToken"/>，用于指示客户端是否已关闭。  
    /// </summary>  
    CancellationToken ClosedToken { get; }

    /// <summary>  
    /// 关闭客户端。  
    /// </summary>  
    /// <param name="msg">关闭时的提示信息。</param>  
    /// <param name="token">可取消令箭。</param>  
    /// <returns>表示异步操作结果的 <see cref="Task{Result}"/>。</returns>  
    Task<Result> CloseAsync(string msg, CancellationToken token = default);
}