// ------------------------------------------------------------------------------
// 此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
// 源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
// CSDN博客：https://blog.csdn.net/qq_40374647
// 哔哩哔哩视频：https://space.bilibili.com/94253567
// Gitee源代码仓库：https://gitee.com/RRQM_Home
// Github源代码仓库：https://github.com/RRQM
// API首页：https://touchsocket.net/
// 交流QQ群：234762506
// 感谢您的下载和使用
// ------------------------------------------------------------------------------

using System;

namespace TouchSocket.Rpc;

/// <summary>
/// RpcServer
/// </summary>
[Obsolete("此类因为不能正确表达Rpc服务的生命周期，已被弃用，请使用SingletonRpcServer直接代替。或者可以选择ScopedRpcServer、TransientRpcServer其他生命周期的服务。", true)]
public class RpcServer
{
}
