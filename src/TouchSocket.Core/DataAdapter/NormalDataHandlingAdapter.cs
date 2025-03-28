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

namespace TouchSocket.Core;

/// <summary>
/// 普通Tcp数据处理器，该适配器不对数据做任何处理。
/// </summary>
public sealed class NormalDataHandlingAdapter : SingleStreamDataHandlingAdapter
{
    /// <inheritdoc/>
    public override bool CanSplicingSend => false;

    /// <inheritdoc/>
    public override bool CanSendRequestInfo => false;

    /// <summary>
    /// 当接收到数据时处理数据
    /// </summary>
    /// <param name="byteBlock">数据流</param>
    protected override async Task PreviewReceivedAsync(ByteBlock byteBlock)
    {
        await this.GoReceivedAsync(byteBlock, null).ConfigureAwait(EasyTask.ContinueOnCapturedContext);
    }

    ///// <summary>
    ///// <inheritdoc/>
    ///// </summary>
    ///// <param name="buffer">数据</param>
    ///// <param name="offset">偏移</param>
    ///// <param name="length">长度</param>
    //protected override void PreviewSend(byte[] buffer, int offset, int length)
    //{
    //    this.GoSend(buffer, offset, length);
    //}

    /// <inheritdoc/>
    protected override Task PreviewSendAsync(ReadOnlyMemory<byte> memory)
    {
        return this.GoSendAsync(memory);
    }

    /// <inheritdoc/>
    protected override void Reset()
    {
        base.Reset();
    }
}