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

using TouchSocket.Core;

namespace TouchSocket.Sockets;

/// <summary>
/// 字节事件参数类，用于在插件之间传递字节块数据
/// </summary>
public class ByteBlockEventArgs : PluginEventArgs
{
    /// <summary>
    /// 初始化字节事件参数对象
    /// </summary>
    /// <param name="byteBlock">需要传递的字节块数据</param>
    public ByteBlockEventArgs(IByteBlockReader byteBlock)
    {
        this.ByteBlock = byteBlock;
    }

    /// <summary>
    /// 获取字节块数据
    /// </summary>
    public IByteBlockReader ByteBlock { get;}
}