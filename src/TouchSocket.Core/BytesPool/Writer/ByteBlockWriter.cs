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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TouchSocket.Core;

[DebuggerDisplay("Length={Length},Position={Position},Capacity={Capacity}")]
public struct ByteBlockWriter : IByteBlockWriter, IDisposable
{
    private Memory<byte> m_memory;
    private readonly Func<int, Memory<byte>> m_onRent;
    private readonly Action<Memory<byte>> m_onReturn;
    private int m_length;

    public ByteBlockWriter(Memory<byte> memory)
    {
        this.m_memory = memory;
    }

    public ByteBlockWriter(int capacity, Func<int, Memory<byte>> onRent, Action<Memory<byte>> onReturn)
    {
        capacity = Math.Max(capacity, 1024);
        this.m_memory = onRent(capacity);
        this.m_onRent = onRent;
        this.m_onReturn = onReturn;
        this.m_length = 0;
    }

    public ByteBlockWriter(int capacity)
    {
        capacity = Math.Max(capacity, 1024);
        this.m_onRent = (c) =>
        {
            return ArrayPool<byte>.Shared.Rent(c);
        };
        this.m_onReturn = (m) =>
        {
            if (MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)m, out var result))
            {
                ArrayPool<byte>.Shared.Return(result.Array);
            }
        };

        this.m_memory = this.m_onRent(capacity);
    }

    public readonly bool IsEmpty => this.m_memory.IsEmpty;
    public readonly int Capacity => this.m_memory.Length;

    public readonly int FreeLength => this.Capacity - this.Position;

    public readonly Memory<byte> TotalMemory => this.m_memory;

    public readonly bool CanSeek => true;

    public readonly bool IsStruct => true;

    public readonly int Length => this.m_length;

    public readonly ReadOnlyMemory<byte> Memory => this.m_memory.Slice(0, this.m_length);

    public int Position { get; set; }

    public readonly ReadOnlySpan<byte> Span => this.Memory.Span;

    public void Advance(int count)
    {
        this.Position += count;
        this.m_length = this.Position > this.m_length ? this.Position : this.m_length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExtendSize(int size)
    {
        if (this.FreeLength < size)
        {
            var need = this.Capacity + size - (this.Capacity - this.Position);
            long lend = this.Capacity;
            while (need > lend)
            {
                lend *= 2;
            }

            if (lend > int.MaxValue)
            {
                lend = Math.Min(need + 1024 * 1024 * 100, int.MaxValue);
            }

            if (this.m_onRent == null || this.m_onReturn == null)
            {
                ThrowHelper.ThrowNotSupportedException("不支持扩容");
                return;
            }


            var bytes = this.m_onRent((int)lend);

            this.m_memory.CopyTo(bytes);

            this.m_onReturn(this.m_memory);
            this.m_memory = bytes;
        }
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        this.ExtendSize(sizeHint);
        return this.m_memory.Slice(this.Position, FreeLength);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        return this.GetMemory(sizeHint).Span;
    }

    public void SeekToEnd()
    {
        this.Position = this.m_length;
    }

    public void SeekToStart()
    {
        this.Position = 0;
    }

    public void SetLength(int value)
    {
        if (value > this.m_memory.Length)
        {
            ThrowHelper.ThrowException("设置的长度超过了内存的长度。");
        }
        this.m_length = value;
    }

    public void Write(scoped ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return;
        }

        this.ExtendSize(span.Length);

        var currentSpan = GetCurrentSpan();
        span.CopyTo(currentSpan);
        this.Position += span.Length;
        this.m_length = Math.Max(this.Position, this.m_length);
    }

    public void WriteBoolean(bool value)
    {
        this.WriteValue<bool>(value);
    }

    public void WriteBooleans(scoped ReadOnlySpan<bool> values)
    {
        if (values.IsEmpty)
        {
            return;
        }
        var size = TouchSocketBitConverter.GetConvertedLength<bool, byte>(values.Length);
        this.ExtendSize(size);

        var currentSpan = this.GetCurrentSpan();
        TouchSocketBitConverter.ConvertValues<bool, byte>(values, currentSpan);
        this.Position += size;
        this.m_length = this.Position > this.m_length ? this.Position : this.m_length;
    }

    public void WriteByte(byte value)
    {
        this.WriteValue<byte>(value);
    }

    public void WriteByteBlock(ByteBlock byteBlock)
    {
        if (byteBlock is null)
        {
            this.WriteVarUInt32(0);
        }
        else
        {
            this.WriteVarUInt32((uint)(byteBlock.Length + 1));
            this.Write(byteBlock.Span);
        }
    }

    public void WriteBytesPackage(scoped ReadOnlySpan<byte> span)
    {
        this.WriteVarUInt32((uint)span.Length);
        this.Write(span);
    }

    public void WriteChar(char value)
    {
        this.WriteValue<char>(value);
    }

    public void WriteChar(char value, EndianType endianType)
    {
        this.WriteValue<char>(value, endianType);
    }

    public void WriteDateTime(DateTime value)
    {
        this.WriteValue<DateTime>(value, EndianType.Big);
    }

    public void WriteDecimal(decimal value)
    {
        this.WriteValue<decimal>(value);
    }

    public void WriteDecimal(decimal value, EndianType endianType)
    {
        this.WriteValue<decimal>(value, endianType);
    }

    public void WriteDouble(double value)
    {
        this.WriteValue<double>(value);
    }

    public void WriteDouble(double value, EndianType endianType)
    {
        this.WriteValue<double>(value, endianType);
    }

    public void WriteFloat(float value)
    {
        this.WriteValue<float>(value);
    }

    public void WriteFloat(float value, EndianType endianType)
    {
        this.WriteValue<float>(value, endianType);
    }

    public void WriteGuid(in Guid value)
    {
        this.WriteValue<Guid>(value, EndianType.Big);
    }

    public void WriteInt16(short value)
    {
        this.WriteValue<short>(value);
    }

    public void WriteInt16(short value, EndianType endianType)
    {
        this.WriteValue<short>(value, endianType);
    }

    public void WriteInt32(int value)
    {
        this.WriteValue<int>(value);
    }

    public void WriteInt32(int value, EndianType endianType)
    {
        this.WriteValue<int>(value, endianType);
    }

    public void WriteInt64(long value)
    {
        this.WriteValue<long>(value);
    }

    public void WriteInt64(long value, EndianType endianType)
    {
        this.WriteValue<long>(value, endianType);
    }

    public void WriteIsNull<T>(T t) where T : class
    {
        if (t == null)
        {
            this.WriteNull();
        }
        else
        {
            this.WriteNotNull();
        }
    }

    public void WriteIsNull<T>(T? t) where T : struct
    {
        if (t.HasValue)
        {
            this.WriteNotNull();
        }
        else
        {
            this.WriteNull();
        }
    }

    public void WriteNormalString(string value, Encoding encoding)
    {
        ThrowHelper.ThrowArgumentNullExceptionIf(value, nameof(value));
        var maxSize = encoding.GetMaxByteCount(value.Length);
        this.ExtendSize(maxSize);
        var chars = value.AsSpan();

        unsafe
        {
            fixed (char* p = &chars[0])
            {
                fixed (byte* p1 = &this.GetCurrentSpan()[0])
                {
                    var len = Encoding.UTF8.GetBytes(p, chars.Length, p1, maxSize);

                    this.Position += len;

                    this.m_length = Math.Max(this.Position, this.m_length);
                }

            }
        }
    }

    public void WriteNotNull()
    {
        this.WriteByte(1);
    }

    public void WriteNull()
    {
        this.WriteByte(0);
    }

    public void WriteString(string value, FixedHeaderType headerType = FixedHeaderType.Int)
    {
        if (value == null)
        {
            switch (headerType)
            {
                case FixedHeaderType.Byte:
                    this.WriteByte(byte.MaxValue);
                    return;
                case FixedHeaderType.Ushort:
                    this.WriteUInt16(ushort.MaxValue);
                    return;
                case FixedHeaderType.Int:
                default:
                    this.WriteInt32(int.MaxValue);
                    return;
            }

        }
        else if (value == string.Empty)
        {
            switch (headerType)
            {
                case FixedHeaderType.Byte:
                    this.WriteByte(0);
                    return;
                case FixedHeaderType.Ushort:
                    this.WriteUInt16(0);
                    return;
                case FixedHeaderType.Int:
                default:
                    this.WriteInt32(0);
                    return;
            }
        }
        else
        {
            var maxSize = (value.Length + 1) * 3 + 4;
            this.ExtendSize(maxSize);
            var chars = value.AsSpan();

            var offset = headerType switch
            {
                FixedHeaderType.Byte => (byte)1,
                FixedHeaderType.Ushort => (byte)2,
                _ => (byte)4,
            };

            var pos = this.Position;

            //this.m_position += offset;

            unsafe
            {
                fixed (char* p = &chars[0])
                {
                    fixed (byte* p1 = &this.GetCurrentSpan()[offset])
                    {
                        var len = Encoding.UTF8.GetBytes(p, chars.Length, p1, maxSize);

                        switch (headerType)
                        {
                            case FixedHeaderType.Byte:
                                if (len >= byte.MaxValue)
                                {
                                    ThrowHelper.ThrowArgumentOutOfRangeException_MoreThan(nameof(value), len, byte.MaxValue);
                                }

                                this.WriteByte((byte)len);
                                break;
                            case FixedHeaderType.Ushort:
                                if (len >= ushort.MaxValue)
                                {
                                    ThrowHelper.ThrowArgumentOutOfRangeException_MoreThan(nameof(value), len, ushort.MaxValue);
                                }
                                this.WriteUInt16((ushort)len);
                                break;
                            case FixedHeaderType.Int:
                            default:
                                if (len >= int.MaxValue)
                                {
                                    ThrowHelper.ThrowArgumentOutOfRangeException_MoreThan(nameof(value), len, int.MaxValue);
                                }
                                this.WriteInt32(len);
                                break;
                        }

                        this.Position += len;

                        this.m_length = Math.Max(this.Position, this.m_length);
                    }

                }
            }
        }
    }

    public void WriteTimeSpan(TimeSpan value)
    {
       this.WriteValue<TimeSpan>(value, EndianType.Big);
    }

    public void WriteUInt16(ushort value)
    {
        this.WriteValue<ushort>(value);
    }

    public void WriteUInt16(ushort value, EndianType endianType)
    {
        this.WriteValue<ushort>(value, endianType);
    }

    public void WriteUInt32(uint value)
    {
        this.WriteValue<uint>(value);
    }

    public void WriteUInt32(uint value, EndianType endianType)
    {
        this.WriteValue<uint>(value, endianType);
    }

    public void WriteUInt64(ulong value)
    {
        this.WriteValue<ulong>(value);
    }

    public void WriteUInt64(ulong value, EndianType endianType)
    {
        this.WriteValue<ulong>(value, endianType);
    }

    public void WriteValue<T>(T value) where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        this.ExtendSize(size);
        TouchSocketBitConverter.Default.WriteBytes(this.GetCurrentSpan(), value);
        this.Position += size;
        this.m_length = this.Position > this.m_length ? this.Position : this.m_length;
    }

    public void WriteValue<T>(T value, EndianType endianType) where T : unmanaged
    {
        var size = Unsafe.SizeOf<T>();
        this.ExtendSize(size);
        TouchSocketBitConverter.GetBitConverter(endianType).WriteBytes(this.GetCurrentSpan(), value);
        this.Position += size;
        this.m_length = this.Position > this.m_length ? this.Position : this.m_length;
    }

    private readonly Span<byte> GetCurrentSpan()
    {
        return this.m_memory.Span.Slice(this.Position);
    }

    public int WriteVarUInt32(uint value)
    {
        this.ExtendSize(5);

        byte bytelength = 0;
        while (value > 0x7F)
        {
            //127=0x7F=0b01111111，大于说明msb=1，即后续还有字节
            var temp = value & 0x7F; //得到数值的后7位,0x7F=0b01111111,0与任何数与都是0,1与任何数与还是任何数
            temp |= 0x80; //后7位不变最高位固定为1,0x80=0b10000000,1与任何数或都是1，0与任何数或都是任何数
            this.m_memory.Span[this.Position++] = (byte)temp; //存储msb=1的数据
            value >>= 7; //右移已经计算过的7位得到下次需要计算的数值
            bytelength++;
        }
        this.m_memory.Span[this.Position++] = (byte)value; //最后一个字节msb=0

        this.m_length = this.Position > this.m_length ? this.Position : this.m_length;
        return bytelength + 1;
    }

    public void Dispose()
    {
        var memory = this.m_memory;
        if (memory.IsEmpty)
        {
            return;
        }

        this.m_memory = default;
        this.m_length = 0;
        this.Position = 0;

        this.m_onReturn?.Invoke(memory);
    }
}
