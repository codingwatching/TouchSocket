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

namespace TouchSocket.Modbus;

/// <summary>
/// ModbusTcpRequest
/// </summary>
internal sealed class ModbusTcpRequest : ModbusTcpBase, IRequestInfoBuilder, IRequestInfo
{
    /// <summary>
    /// 从<see cref="ModbusRequest"/>创建一个ModbusTcpRequest
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="request"></param>
    public ModbusTcpRequest(ushort transactionId, ModbusRequest request)
    {
        this.TransactionId = transactionId;
        this.ProtocolId = 0;
        this.SlaveId = request.SlaveId;
        this.FunctionCode = request.FunctionCode;
        this.StartingAddress = request.StartingAddress;
        this.Quantity = request.Quantity;
        this.Data = request.Data;
        this.ReadStartAddress = request.ReadStartAddress;
        this.ReadQuantity = request.ReadQuantity;
    }

    /// <inheritdoc/>
    public int MaxLength => 1024;

    /// <inheritdoc/>
    public void Build<TWriter>(ref TWriter writer) 
        where TWriter : IByteBlockWriter
#if AllowsRefStruct
,allows ref struct
#endif
    {
        writer.WriteUInt16(this.TransactionId, EndianType.Big);
        writer.WriteUInt16(this.ProtocolId, EndianType.Big);
        if ((byte)this.FunctionCode <= 4)
        {
            writer.WriteUInt16(6, EndianType.Big);
            writer.WriteByte(this.SlaveId);
            writer.WriteByte((byte)this.FunctionCode);
            writer.WriteUInt16(this.StartingAddress, EndianType.Big);
            writer.WriteUInt16(this.Quantity, EndianType.Big);
        }
        else if (this.FunctionCode == FunctionCode.WriteSingleCoil || this.FunctionCode == FunctionCode.WriteSingleRegister)
        {
            writer.WriteUInt16(6, EndianType.Big);
            writer.WriteByte(this.SlaveId);
            writer.WriteByte((byte)this.FunctionCode);
            writer.WriteUInt16(this.StartingAddress, EndianType.Big);
            writer.Write(this.Data.Span);
        }
        else if (this.FunctionCode == FunctionCode.WriteMultipleCoils || this.FunctionCode == FunctionCode.WriteMultipleRegisters)
        {
            writer.WriteUInt16((ushort)(this.Data.Length + 7), EndianType.Big);
            writer.WriteByte(this.SlaveId);
            writer.WriteByte((byte)this.FunctionCode);
            writer.WriteUInt16(this.StartingAddress, EndianType.Big);
            writer.WriteUInt16(this.Quantity, EndianType.Big);
            writer.WriteByte((byte)this.Data.Length);
            writer.Write(this.Data.Span);
        }
        else if (this.FunctionCode == FunctionCode.ReadWriteMultipleRegisters)
        {
            writer.WriteUInt16((ushort)(this.Data.Length + 11), EndianType.Big);
            writer.WriteByte(this.SlaveId);
            writer.WriteByte((byte)this.FunctionCode);
            writer.WriteUInt16(this.ReadStartAddress, EndianType.Big);
            writer.WriteUInt16(this.ReadQuantity, EndianType.Big);
            writer.WriteUInt16(this.StartingAddress, EndianType.Big);
            writer.WriteUInt16(this.Quantity, EndianType.Big);
            writer.WriteByte((byte)this.Data.Length);
            writer.Write(this.Data.Span);
        }
        else
        {
            throw new System.InvalidOperationException("无法识别的功能码");
        }
    }
}