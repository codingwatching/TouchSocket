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
using System.Buffers;
using System.Text;

namespace TouchSocket.Core;

public interface IBytesWriter: IBytesCore
{
  
    void Write(scoped ReadOnlySpan<byte> span);

    void WriteBoolean(bool value);

    void WriteBooleans(scoped ReadOnlySpan<bool> values);

    void WriteByte(byte value);

    void WriteByteBlock(ByteBlock byteBlock);

    void WriteBytesPackage(scoped ReadOnlySpan<byte> span);

    void WriteChar(char value);

    void WriteChar(char value, EndianType endianType);

    void WriteDateTime(DateTime value);

    void WriteDecimal(decimal value);

    void WriteDecimal(decimal value, EndianType endianType);

    void WriteDouble(double value);

    void WriteDouble(double value, EndianType endianType);

    void WriteFloat(float value);

    void WriteFloat(float value, EndianType endianType);

    void WriteGuid(in Guid value);

    void WriteInt16(short value);

    void WriteInt16(short value, EndianType endianType);

    void WriteInt32(int value);

    void WriteInt32(int value, EndianType endianType);

    void WriteInt64(long value);

    void WriteInt64(long value, EndianType endianType);

    void WriteIsNull<T>(T t) where T : class;

    void WriteIsNull<T>(T? t) where T : struct;

    void WriteNormalString(string value, Encoding encoding);

    void WriteNotNull();

    void WriteNull();

    void WriteString(string value, FixedHeaderType headerType = FixedHeaderType.Int);

    void WriteTimeSpan(TimeSpan value);

    void WriteUInt16(ushort value);

    void WriteUInt16(ushort value, EndianType endianType);

    void WriteUInt32(uint value);

    void WriteUInt32(uint value, EndianType endianType);

    void WriteUInt64(ulong value);

    void WriteUInt64(ulong value, EndianType endianType);

    void WriteValue<T>(T value) where T : unmanaged;

    void WriteValue<T>(T value, EndianType endianType) where T : unmanaged;

    int WriteVarUInt32(uint value);
}