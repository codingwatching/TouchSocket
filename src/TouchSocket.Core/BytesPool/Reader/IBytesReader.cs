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
using System.Collections.Generic;

namespace TouchSocket.Core;

public interface IBytesReader : IBytesCore
{
    bool CanRead { get; }
    int CanReadLength { get; }

    int Read(Span<byte> span);

    bool ReadBoolean();

    ReadOnlyMemory<bool> ReadBooleans();

    byte ReadByte();

    ByteBlock ReadByteBlock();

    ReadOnlySpan<byte> ReadBytesPackageSpan();

    char ReadChar();

    char ReadChar(EndianType endianType);

    DateTime ReadDateTime();

    decimal ReadDecimal();

    decimal ReadDecimal(EndianType endianType);

    double ReadDouble();

    double ReadDouble(EndianType endianType);

    float ReadFloat();

    float ReadFloat(EndianType endianType);

    Guid ReadGuid();

    short ReadInt16();

    short ReadInt16(EndianType endianType);

    int ReadInt32();

    int ReadInt32(EndianType endianType);

    long ReadInt64();

    long ReadInt64(EndianType endianType);

    bool ReadIsNull();

    string ReadString(FixedHeaderType headerType = FixedHeaderType.Int);

    TimeSpan ReadTimeSpan();

    ReadOnlySpan<byte> ReadToSpan(int length);

    ushort ReadUInt16();

    ushort ReadUInt16(EndianType endianType);

    uint ReadUInt32();

    uint ReadUInt32(EndianType endianType);

    ulong ReadUInt64();

    ulong ReadUInt64(EndianType endianType);

    T ReadValue<T>() where T : unmanaged;

    T ReadValue<T>(EndianType endianType) where T : unmanaged;

    uint ReadVarUInt32();
}