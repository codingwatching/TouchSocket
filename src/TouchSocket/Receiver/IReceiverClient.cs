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

namespace TouchSocket.Sockets;


/// <summary>
/// 定义了一个泛型接口，用于创建和管理同步数据接收器客户端
/// </summary>
/// <typeparam name="TResult">接收结果的类型，必须继承自<see cref="IReceiverResult"/>接口</typeparam>
public interface IReceiverClient<TResult> where TResult : IReceiverResult
{
    /// <summary>
    /// 获取一个同步数据接收器
    /// </summary>
    /// <returns>返回一个IReceiver接口实例，用于接收类型为TResult的数据</returns>
    IReceiver<TResult> CreateReceiver();

    /// <summary>
    /// 移除同步数据接收器
    /// </summary>
    void ClearReceiver();
}