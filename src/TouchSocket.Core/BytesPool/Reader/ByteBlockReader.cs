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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TouchSocket.Core;

[DebuggerDisplay("Length={Length},Position={Position}")]
public struct ByteBlockReader : IByteBlockReader
{
    private readonly ReadOnlyMemory<byte> m_memory;

    public ByteBlockReader(ReadOnlyMemory<byte> memory)
    {
        this.m_memory = memory;
    }

    public readonly bool CanRead => this.CanReadLength > 0;

    public readonly int CanReadLength => this.Length - this.Position;

    public readonly bool CanSeek => true;

    public readonly bool IsStruct => true;

    public readonly int Length => this.m_memory.Length;

    public readonly ReadOnlyMemory<byte> Memory => this.m_memory;

    public int Position { get; set; }

    public readonly ReadOnlySpan<byte> Span => this.m_memory.Span;

    public int Read(Span<byte> span)
    {
        if (span.IsEmpty)
        {
            return 0;
        }

        var length = Math.Min(this.CanReadLength, span.Length);

        this.Span.Slice(this.Position, length).CopyTo(span);
        this.Position += length;
        return length;
    }

    public bool ReadBoolean()
    {
        return this.ReadValue<bool>();
    }

    public ReadOnlyMemory<bool> ReadBooleans()
    {
        var size = 1;
        if (this.CanReadLength < size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_LessThan(nameof(size), this.CanReadLength, size);
        }

        var value = TouchSocketBitConverter.Default.ToValues<bool>(this.Span.Slice(this.Position, size));
        this.Position += size;
        return value;
    }

    public byte ReadByte()
    {
        var size = Unsafe.SizeOf<byte>();
        if (this.CanReadLength < size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_LessThan(nameof(size), this.CanReadLength, size);
        }

        var value = this.Span[this.Position];
        this.Position += size;
        return value;
    }

    public ByteBlock ReadByteBlock()
    {
        var len = (int)this.ReadVarUInt32() - 1;

        if (len < 0)
        {
            return default;
        }

        var byteBlock = new ByteBlock(len);
        byteBlock.Write(this.Span.Slice(this.Position, len));
        byteBlock.SeekToStart();
        this.Position += len;
        return byteBlock;
    }

    public ReadOnlySpan<byte> ReadBytesPackageSpan()
    {
        var length = this.ReadVarUInt32();
        if (length < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_LessThan(nameof(length), length, 0);
        }

        var span = this.Span.Slice(this.Position, (int)length);
        this.Position += (int)length;
        return span;
    }

    public char ReadChar()
    {
        return this.ReadValue<char>();
    }

    public char ReadChar(EndianType endianType)
    {
        return this.ReadValue<char>(endianType);
    }

    public DateTime ReadDateTime()
    {
        return this.ReadValue<DateTime>( EndianType.Big);
    }

    public decimal ReadDecimal()
    {
        return this.ReadValue<decimal>();
    }

    public decimal ReadDecimal(EndianType endianType)
    {
        return this.ReadValue<decimal>(endianType);
    }

    public double ReadDouble()
    {
        return this.ReadValue<double>();
    }

    public double ReadDouble(EndianType endianType)
    {
        return this.ReadValue<double>(endianType);
    }

    public float ReadFloat()
    {
        return this.ReadValue<float>();
    }

    public float ReadFloat(EndianType endianType)
    {
        return this.ReadValue<float>(endianType);
    }

    public Guid ReadGuid()
    {
        return this.ReadValue<Guid>( EndianType.Big);
    }

    public short ReadInt16()
    {
        return this.ReadValue<short>();
    }

    public short ReadInt16(EndianType endianType)
    {
        return this.ReadValue<short>(endianType);
    }

    public int ReadInt32()
    {
        return this.ReadValue<int>();
    }

    public int ReadInt32(EndianType endianType)
    {
        return this.ReadValue<int>(endianType);
    }

    public long ReadInt64()
    {
        return this.ReadValue<long>();
    }

    public long ReadInt64(EndianType endianType)
    {
        return this.ReadValue<long>(endianType);
    }

    public bool ReadIsNull()
    {
        var status = this.ReadByte();
        return status == 0 || (status == 1 ? false : throw new Exception("标识既非Null，也非NotNull，可能是流位置发生了错误。"));
    }

    public string ReadString(FixedHeaderType headerType = FixedHeaderType.Int)
    {
        int len;
        switch (headerType)
        {
            case FixedHeaderType.Byte:
                len = this.ReadByte();
                if (len == byte.MaxValue)
                {
                    return null;
                }
                break;
            case FixedHeaderType.Ushort:
                len = this.ReadUInt16();
                if (len == ushort.MaxValue)
                {
                    return null;
                }
                break;
            case FixedHeaderType.Int:
            default:
                len = this.ReadInt32();
                if (len == int.MaxValue)
                {
                    return null;
                }
                break;
        }

        var str = this.Span.Slice(this.Position, len).ToString(Encoding.UTF8);
        this.Position += len;
        return str;
    }

    public TimeSpan ReadTimeSpan()
    {
        return this.ReadValue<TimeSpan>( EndianType.Big);
    }

    public ReadOnlySpan<byte> ReadToSpan(int length)
    {
        var span = this.Span.Slice(this.Position, length);
        this.Position += length;
        return span;
    }

    public ushort ReadUInt16()
    {
        return this.ReadValue<ushort>();
    }

    public ushort ReadUInt16(EndianType endianType)
    {
        return this.ReadValue<ushort>(endianType);
    }

    public uint ReadUInt32()
    {
        return this.ReadValue<uint>();
    }

    public uint ReadUInt32(EndianType endianType)
    {
        return this.ReadValue<uint>(endianType);
    }

    public ulong ReadUInt64()
    {
        return this.ReadValue<ulong>();
    }

    public ulong ReadUInt64(EndianType endianType)
    {
        return this.ReadValue<ulong>(endianType);
    }

    public T ReadValue<T>() where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        if (this.CanReadLength < size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_LessThan(nameof(size), this.CanReadLength, size);
        }
        var value = TouchSocketBitConverter.Default.To<T>(this.Span.Slice(this.Position));
        this.Position += size;
        return value;
    }

    public T ReadValue<T>(EndianType endianType) where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        if (this.CanReadLength < size)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException_LessThan(nameof(size), this.CanReadLength, size);
        }
        var value = TouchSocketBitConverter.GetBitConverter(endianType).To<T>(this.Span.Slice(this.Position));
        this.Position += size;
        return value;
    }

    public uint ReadVarUInt32()
    {
        uint value = 0;
        var byteLength = 0;
        while (true)
        {
            var b = this.Span[this.Position++];
            var temp = (b & 0x7F); //取每个字节的后7位
            temp <<= (7 * byteLength); //向左移位，越是后面的字节，移位越多
            value += (uint)temp; //把每个字节的值加起来就是最终的值了
            byteLength++;
            if (b <= 0x7F)
            { //127=0x7F=0b01111111，小于等于说明msb=0，即最后一个字节
                break;
            }
        }
        return value;
    }

    public void SeekToEnd()
    {
        this.Position = this.Length;
    }

    public void SeekToStart()
    {
        this.Position = 0;
    }
}
