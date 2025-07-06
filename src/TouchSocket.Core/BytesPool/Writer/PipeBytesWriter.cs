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

#if NET9_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace TouchSocket.Core;
internal readonly struct PipeBytesWriter
{
    private readonly PipeWriter m_writer;

    public PipeBytesWriter(PipeWriter writer)
    {
        this.m_writer = writer;
    }

    public void Write<T>(T value, EndianType endianType) where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        var span = this.m_writer.GetSpan(size);
        var converter = TouchSocketBitConverter.GetBitConverter(endianType);
        converter.WriteBytes(span, value);
        this.m_writer.Advance(size);
    }

    public ValueTask<FlushResult> FlushAsync(CancellationToken token)
    {
        return this.m_writer.FlushAsync(token);
    }
}

#endif
