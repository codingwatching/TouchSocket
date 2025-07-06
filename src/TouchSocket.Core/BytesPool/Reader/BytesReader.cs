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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace TouchSocket.Core;


[DebuggerDisplay("Length={Length},Position={Position}")]
public ref struct BytesReader : IBytesReader
{
    private readonly ReadOnlySpan<byte> m_span;

    public BytesReader(ReadOnlySpan<byte> span)
    {
        this.m_span = span;
    }

    /// <inheritdoc/>
    public readonly bool CanRead => this.CanReadLength > 0;

    /// <inheritdoc/>
    public readonly int CanReadLength => this.Length - this.Position;

    /// <inheritdoc/>
    public readonly bool CanSeek => true;

    /// <inheritdoc/>
    public readonly bool IsStruct => true;

    /// <inheritdoc/>
    public readonly int Length => this.m_span.Length;

    /// <inheritdoc/>
    public int Position { get; set; }

    /// <inheritdoc/>
    public readonly ReadOnlySpan<byte> Span => this.m_span;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool ReadBoolean()
    {
        return this.ReadValue<bool>();
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public char ReadChar()
    {
        return this.ReadValue<char>();
    }

    /// <inheritdoc/>
    public char ReadChar(EndianType endianType)
    {
        return this.ReadValue<char>(endianType);
    }

    /// <inheritdoc/>
    public DateTime ReadDateTime()
    {
        return this.ReadValue<DateTime>(EndianType.Big);
    }

    /// <inheritdoc/>
    public decimal ReadDecimal()
    {
        return this.ReadValue<decimal>();
    }

    /// <inheritdoc/>
    public decimal ReadDecimal(EndianType endianType)
    {
        return this.ReadValue<decimal>(endianType);
    }

    /// <inheritdoc/>
    public double ReadDouble()
    {
        return this.ReadValue<double>();
    }

    /// <inheritdoc/>
    public double ReadDouble(EndianType endianType)
    {
        return this.ReadValue<double>(endianType);
    }

    /// <inheritdoc/>
    public float ReadFloat()
    {
        return this.ReadValue<float>();
    }

    /// <inheritdoc/>
    public float ReadFloat(EndianType endianType)
    {
        return this.ReadValue<float>(endianType);
    }

    /// <inheritdoc/>
    public Guid ReadGuid()
    {
        return this.ReadValue<Guid>(EndianType.Big);
    }

    /// <inheritdoc/>
    public short ReadInt16()
    {
        return this.ReadValue<short>();
    }

    /// <inheritdoc/>
    public short ReadInt16(EndianType endianType)
    {
        return this.ReadValue<short>(endianType);
    }

    /// <inheritdoc/>
    public int ReadInt32()
    {
        return this.ReadValue<int>();
    }

    /// <inheritdoc/>
    public int ReadInt32(EndianType endianType)
    {
        return this.ReadValue<int>(endianType);
    }

    /// <inheritdoc/>
    public long ReadInt64()
    {
        return this.ReadValue<long>();
    }

    /// <inheritdoc/>
    public long ReadInt64(EndianType endianType)
    {
        return this.ReadValue<long>(endianType);
    }

    /// <inheritdoc/>
    public bool ReadIsNull()
    {
        var status = this.ReadByte();
        return status == 0 || (status == 1 ? false : throw new Exception("标识既非Null，也非NotNull，可能是流位置发生了错误。"));
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public TimeSpan ReadTimeSpan()
    {
        return this.ReadValue<TimeSpan>(EndianType.Big);
    }

    /// <inheritdoc/>
    public ReadOnlySpan<byte> ReadToSpan(int length)
    {
        var span = this.Span.Slice(this.Position, length);
        this.Position += length;
        return span;
    }

    /// <inheritdoc/>
    public ushort ReadUInt16()
    {
        return this.ReadValue<ushort>();
    }

    /// <inheritdoc/>
    public ushort ReadUInt16(EndianType endianType)
    {
        return this.ReadValue<ushort>(endianType);
    }

    /// <inheritdoc/>
    public uint ReadUInt32()
    {
        return this.ReadValue<uint>();
    }

    /// <inheritdoc/>
    public uint ReadUInt32(EndianType endianType)
    {
        return this.ReadValue<uint>(endianType);
    }

    /// <inheritdoc/>
    public ulong ReadUInt64()
    {
        return this.ReadValue<ulong>();
    }

    /// <inheritdoc/>
    public ulong ReadUInt64(EndianType endianType)
    {
        return this.ReadValue<ulong>(endianType);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void SeekToEnd()
    {
        this.Position = this.Length;
    }

    /// <inheritdoc/>
    public void SeekToStart()
    {
        this.Position = 0;
    }
}