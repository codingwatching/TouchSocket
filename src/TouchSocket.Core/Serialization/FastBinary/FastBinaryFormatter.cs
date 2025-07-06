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
using System.Buffers;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace TouchSocket.Core;

/// <summary>
/// 快速二进制序列化。
/// </summary>
public static class FastBinaryFormatter
{
    /// <summary>
    /// 定义一个常量，指定动态访问的成员类型，包括公共构造函数、方法、字段和属性。
    /// </summary>
    public const DynamicallyAccessedMemberTypes DynamicallyAccessed = DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties;

    /// <summary>
    /// 初始化一个默认的快速序列化上下文对象，用于支持快速序列化操作。
    /// </summary>
    private static readonly DefaultFastSerializerContext s_defaultFastSerializerContext = new DefaultFastSerializerContext();

    /// <summary>
    /// 获取默认的快速序列化上下文。
    /// </summary>
    public static FastSerializerContext DefaultFastSerializerContext => s_defaultFastSerializerContext;

    /// <summary>
    /// 添加一个新的快速二进制转换器，该转换器由指定的泛型参数提供。
    /// </summary>
    /// <typeparam name="TType">要序列化的类型。</typeparam>
    /// <typeparam name="TConverter">转换器类型，必须实现 <see cref="IFastBinaryConverter"/> 接口。</typeparam>
    /// <remarks>
    /// [DynamicallyAccessedMembers(DynamicallyAccessed)] 特性确保类型和转换器在反射时可被动态访问。
    /// </remarks>
    public static void AddFastBinaryConverter<[DynamicallyAccessedMembers(DynamicallyAccessed)] TType, [DynamicallyAccessedMembers(DynamicallyAccessed)] TConverter>() where TConverter : IFastBinaryConverter, new()
    {
        // 使用 Activator.CreateInstance 创建转换器实例，并将其添加到默认的序列化上下文中
        AddFastBinaryConverter(typeof(TType), (IFastBinaryConverter)Activator.CreateInstance(typeof(TConverter)));
    }

    /// <summary>
    /// 为指定类型添加一个快速二进制转换器。
    /// </summary>
    /// <typeparam name="TType">要序列化的类型。</typeparam>
    /// <param name="converter">要添加的转换器，必须实现 <see cref="IFastBinaryConverter"/> 接口。</param>
    /// <remarks>
    /// [DynamicallyAccessedMembers(DynamicallyAccessed)] 特性确保类型在反射时可被动态访问。
    /// </remarks>
    public static void AddFastBinaryConverter<[DynamicallyAccessedMembers(DynamicallyAccessed)] TType>(IFastBinaryConverter converter)
    {
        // 将转换器添加到默认的序列化上下文中
        AddFastBinaryConverter(typeof(TType), converter);
    }

    /// <summary>
    /// 为指定类型添加一个快速二进制转换器。
    /// </summary>
    /// <param name="type">要序列化的类型。</param>
    /// <param name="converter">要添加的转换器，必须实现 <see cref="IFastBinaryConverter"/> 接口。</param>
    /// <remarks>
    /// [DynamicallyAccessedMembers(DynamicallyAccessed)] 特性确保类型在反射时可被动态访问。
    /// </remarks>
    public static void AddFastBinaryConverter([DynamicallyAccessedMembers(DynamicallyAccessed)] Type type, IFastBinaryConverter converter)
    {
        // 将转换器添加到默认的序列化上下文中
        s_defaultFastSerializerContext.AddFastBinaryConverter(type, converter);
    }

    #region Serialize

    /// <summary>
    /// 序列化给定对象并将其写入字节块。
    /// </summary>
    /// <param name="byteBlock">用于存储序列化数据的字节块。</param>
    /// <param name="graph">要序列化的对象。</param>
    /// <param name="serializerContext">（可选）序列化上下文，提供额外的序列化设置或上下文。</param>
    /// <typeparam name="T">要序列化的对象类型。</typeparam>
    /// <remarks>
    /// 此方法提供了一种便捷的方式来序列化对象至字节块，利用提供的序列化上下文进行序列化过程。
    /// </remarks>
    public static void Serialize<[DynamicallyAccessedMembers(DynamicallyAccessed)] T>(ByteBlock byteBlock, in T graph, FastSerializerContext serializerContext = null)
    {
        Serialize(ref byteBlock, graph, serializerContext);
    }

    /// <summary>
    /// 使用指定的序列化上下文将对象序列化到提供的字节块中。
    /// </summary>
    /// <typeparam name="TWriter">字节块的类型，必须实现IByteBlock接口。</typeparam>
    /// <typeparam name="T">要序列化的对象类型。</typeparam>
    /// <param name="writer">用于存储序列化数据的字节块。</param>
    /// <param name="graph">要序列化的对象。</param>
    /// <param name="serializerContext">用于序列化的上下文。</param>
    public static void Serialize<TWriter, [DynamicallyAccessedMembers(DynamicallyAccessed)] T>(ref TWriter writer, in T graph, FastSerializerContext serializerContext = null)
        where TWriter : IByteBlockWriter
#if AllowsRefStruct
,allows ref struct
#endif
    {
        serializerContext ??= s_defaultFastSerializerContext;

        var startPosition = writer.Position;

        writer.Position = startPosition + 1;
        SerializeObject(ref writer, graph, serializerContext);

        var pos = writer.Position;
        writer.Position = startPosition;
        writer.WriteByte(1);

        writer.Position = pos;
    }

    /// <summary>
    /// 使用指定的序列化上下文将对象序列化为字节数组。
    /// </summary>
    /// <typeparam name="T">要序列化的对象类型。</typeparam>
    /// <param name="graph">要序列化的对象。</param>
    /// <param name="serializerContext">用于序列化的上下文。</param>
    /// <returns>序列化后的字节数组。</returns>
    public static byte[] SerializeToBytes<[DynamicallyAccessedMembers(DynamicallyAccessed)] T>([DynamicallyAccessedMembers(DynamicallyAccessed)] in T graph, FastSerializerContext serializerContext = null)
    {
        var byteBlock = new ValueByteBlock(1024 * 64);
        try
        {
            Serialize(ref byteBlock, graph, serializerContext);
            return byteBlock.ToArray();
        }
        finally
        {
            byteBlock.Dispose();
        }
    }

    private static void SerializeIListOrArray<TWriter>(ref TWriter writer, in IEnumerable param, FastSerializerContext serializerContext) 
        where TWriter : IByteBlockWriter
#if AllowsRefStruct
,allows ref struct
#endif
    {
        var oldPosition = writer.Position;
        writer.Position += 4;
        uint paramLen = 0;

        foreach (var item in param)
        {
            paramLen++;
            SerializeObject(ref writer, item, serializerContext);
        }
        var newPosition = writer.Position;
        writer.Position = oldPosition;
        writer.WriteUInt32(paramLen);
        writer.Position = newPosition;
    }

    private static void SerializeMutilDimensionalArray<TWriter>(ref TWriter writer, Array array, FastSerializerContext serializerContext) 
        where TWriter : IByteBlockWriter
#if AllowsRefStruct
,allows ref struct
#endif
    {
        var rank = array.Rank;
        for (var i = 0; i < rank; i++)
        {
            writer.WriteInt32(array.GetLength(i));
        }

        //var oldPosition = byteBlock.Position;
        //byteBlock.Position += 4;
        //uint paramLen = 0;

        foreach (var item in array)
        {
            //paramLen++;
            SerializeObject(ref writer, item, serializerContext);
        }
        //var newPosition = byteBlock.Position;
        //byteBlock.Position = oldPosition;
        //byteBlock.WriteUInt32(paramLen);
        //byteBlock.Position = newPosition;
    }

    private static void SerializeObject<TWriter, T>(ref TWriter writer, in T graph, FastSerializerContext serializerContext)
        where TWriter : IByteBlockWriter
#if AllowsRefStruct
,allows ref struct
#endif
    {
        if (graph is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteNotNull();
        switch (graph)
        {
            case byte value:
                {
                    writer.WriteByte(value);
                    return;
                }
            case sbyte value:
                {
                    writer.WriteInt16(value);
                    return;
                }
            case bool value:
                {
                    writer.WriteBoolean(value);
                    return;
                }
            case short value:
                {
                    writer.WriteInt16(value);
                    return;
                }
            case ushort value:
                {
                    writer.WriteUInt16(value);
                    return;
                }
            case int value:
                {
                    writer.WriteInt32(value);
                    return;
                }
            case uint value:
                {
                    writer.WriteUInt32(value);
                    return;
                }
            case long value:
                {
                    writer.WriteInt64(value);
                    return;
                }
            case ulong value:
                {
                    writer.WriteUInt64(value);
                    return;
                }
            case float value:
                {
                    writer.WriteFloat(value);
                    return;
                }
            case double value:
                {
                    writer.WriteDouble(value);
                    return;
                }
            case char value:
                {
                    writer.WriteChar(value);
                    return;
                }
            case decimal value:
                {
                    writer.WriteDecimal(value);
                    return;
                }
            case DateTime value:
                {
                    writer.WriteDateTime(value);
                    return;
                }
            case TimeSpan value:
                {
                    writer.WriteTimeSpan(value);
                    return;
                }
            case string value:
                {
                    writer.WriteString(value);
                    return;
                }
            case Enum _:
                {
                    var type = graph.GetType();
                    var enumValType = Enum.GetUnderlyingType(type);

                    if (enumValType == typeof(byte))
                    {
                        writer.WriteByte(Convert.ToByte(graph));
                    }
                    else if (enumValType == typeof(sbyte))
                    {
                        writer.WriteInt16(Convert.ToSByte(graph));
                    }
                    else if (enumValType == typeof(short))
                    {
                        writer.WriteInt16(Convert.ToInt16(graph));
                    }
                    else if (enumValType == typeof(ushort))
                    {
                        writer.WriteUInt16(Convert.ToUInt16(graph));
                    }
                    else if (enumValType == typeof(int))
                    {
                        writer.WriteInt32(Convert.ToInt32(graph));
                    }
                    else if (enumValType == typeof(uint))
                    {
                        writer.WriteUInt32(Convert.ToUInt32(graph));
                    }
                    else if (enumValType == typeof(ulong))
                    {
                        writer.WriteUInt64(Convert.ToUInt64(graph));
                    }
                    else
                    {
                        writer.WriteInt64(Convert.ToInt64(graph));
                    }
                    return;
                }
            case byte[] value:
                {
                    writer.WriteBytesPackage(value);
                    return;
                }
            default:
                {
                    var startPosition = writer.Position;
                    writer.Position += 4;
                    var type = graph.GetType();
                    var serializeObject = serializerContext.GetSerializeObject(type);
                    if (serializeObject.Converter != null)
                    {
                        serializeObject.Converter.Write(ref writer, graph);
                    }
                    else
                    {
                        switch (serializeObject.InstanceType)
                        {
                            case InstanceType.List:
                                SerializeIListOrArray(ref writer, (IEnumerable)graph, serializerContext);
                                break;

                            case InstanceType.Array:
                                var array = (Array)(object)graph;
                                if (array.Rank == 1)
                                {
                                    SerializeIListOrArray(ref writer, array, serializerContext);
                                }
                                else
                                {
                                    SerializeMutilDimensionalArray(ref writer, array, serializerContext);
                                }
                                break;

                            case InstanceType.Dictionary:
                                {
                                    var oldPosition = writer.Position;
                                    writer.Position += 4;
                                    uint paramLen = 0;

                                    foreach (DictionaryEntry item in (IDictionary)graph)
                                    {
                                        SerializeObject(ref writer, item.Key, serializerContext);
                                        SerializeObject(ref writer, item.Value, serializerContext);
                                        paramLen++;
                                    }
                                    var newPosition = writer.Position;
                                    writer.Position = oldPosition;
                                    writer.WriteUInt32(paramLen);
                                    writer.Position = newPosition;
                                }
                                break;

                            default:
                                {
                                    for (var i = 0; i < serializeObject.MemberInfos.Length; i++)
                                    {
                                        var memberInfo = serializeObject.MemberInfos[i];
                                        if (serializeObject.EnableIndex)
                                        {
                                            writer.WriteByte(memberInfo.Index);
                                        }
                                        else
                                        {
                                            writer.WriteString(memberInfo.Name, FixedHeaderType.Byte);
                                        }

                                        SerializeObject(ref writer, serializeObject.MemberAccessor.GetValue(graph, memberInfo.Name), serializerContext);
                                    }
                                }

                                break;
                        }
                    }

                    var endPosition = writer.Position;
                    writer.Position = startPosition;

                    writer.WriteInt32(endPosition - startPosition - 4);
                    writer.Position = endPosition;
                    break;
                }
        }
    }

    #endregion Serialize

    #region Deserialize

    /// <summary>
    /// 反序列化字节块为指定类型的对象。
    /// </summary>
    /// <param name="byteBlock">包含待反序列化数据的字节块。</param>
    /// <param name="serializerContext">（可选）快速序列化上下文，用于优化性能。</param>
    /// <typeparam name="T">要反序列化为的类型。</typeparam>
    /// <returns>反序列化后的对象。</returns>
    public static T Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessed)] T>(ByteBlock byteBlock, FastSerializerContext serializerContext = null)
    {
        // 调用泛型方法 Deserialize，使用 ByteBlock 作为泛型参数，进行反序列化操作
        return Deserialize<ByteBlock, T>(ref byteBlock, serializerContext);
    }
    /// <summary>
    /// 反序列化字节块为指定类型。
    /// </summary>
    /// <param name="byteBlock">包含待反序列化数据的字节块。</param>
    /// <param name="serializerContext">（可选）序列化上下文，用于控制序列化行为。</param>
    /// <typeparam name="T">要反序列化的类型，该类型标记有[DynamicallyAccessedMembers]特性，表示在运行时会动态访问其成员。</typeparam>
    /// <returns>反序列化后的对象。</returns>
    public static T Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessed)] T>(ref ValueByteBlock byteBlock, FastSerializerContext serializerContext = null)
    {
        // 调用泛型方法 Deserialize，使用 ValueByteBlock 作为数据容器，T 为目标类型，进行反序列化操作
        return Deserialize<ValueByteBlock, T>(ref byteBlock, serializerContext);
    }

    /// <summary>
    /// 将字节数组反序列化为指定类型的实例。
    /// </summary>
    /// <param name="bytes">包含已序列化数据的字节数组。</param>
    /// <param name="serializerContext">（可选）用于序列化和反序列化过程的上下文对象。</param>
    /// <typeparam name="T">要反序列化的目标类型。</typeparam>
    /// <returns>反序列化后的目标类型实例。</returns>
    public static T Deserialize<[DynamicallyAccessedMembers(DynamicallyAccessed)] T>(byte[] bytes, FastSerializerContext serializerContext = null)
    {
        // 创建一个包装字节数组的ValueByteBlock对象，以便处理反序列化过程。
        var byteBlock = new ValueByteBlock(bytes);
        // 调用重载的Deserialize方法，使用ValueByteBlock帮助类进行反序列化操作。
        return Deserialize<ValueByteBlock, T>(ref byteBlock, serializerContext);
    }

    /// <summary>
    /// 使用指定的序列化上下文从字节块中反序列化出指定类型的对象。
    /// </summary>
    /// <typeparam name="TReader">实现IByteBlock接口的类型，用于读取字节数据。</typeparam>
    /// <typeparam name="T">要反序列化为的类型。</typeparam>
    /// <param name="reader">包含序列化数据的字节块。</param>
    /// <param name="serializerContext">用于反序列化的FastSerializerContext实例。</param>
    /// <returns>反序列化后的对象。</returns>
    public static T Deserialize<TReader, [DynamicallyAccessedMembers(DynamicallyAccessed)] T>(ref TReader reader, FastSerializerContext serializerContext = null)
        where TReader : IByteBlockReader
#if AllowsRefStruct
,allows ref struct
#endif
    {
        // 调用重载的Deserialize方法，传入类型信息和序列化上下文进行反序列化。
        return (T)Deserialize(ref reader, typeof(T), serializerContext);
    }

    /// <summary>
    /// 使用指定的序列化上下文从字节块中反序列化对象。
    /// </summary>
    /// <typeparam name="TReader">实现IByteBlock接口的类型，用于读取字节数据。</typeparam>
    /// <param name="reader">包含序列化数据的字节块。</param>
    /// <param name="type">要反序列化为的类型。</param>
    /// <param name="serializerContext">用于反序列化的FastSerializerContext实例。</param>
    /// <returns>反序列化后的对象。</returns>
    public static object Deserialize<TReader>(ref TReader reader, [DynamicallyAccessedMembers(DynamicallyAccessed)] Type type, FastSerializerContext serializerContext = null)
        where TReader : IByteBlockReader
#if AllowsRefStruct
,allows ref struct
#endif
    {
        // 检查数据流是否正确，如果不是，则抛出异常。
        if (reader.ReadByte() != 1)
        {
            throw new Exception("Fast反序列化数据流解析错误。");
        }

        serializerContext ??= s_defaultFastSerializerContext;
        // 使用提供的序列化上下文进行反序列化。
        return Deserialize(type, ref reader, serializerContext);
    }

    private static object Deserialize<TReader>(Type type, ref TReader reader, FastSerializerContext serializerContext)
        where TReader : IByteBlockReader
#if AllowsRefStruct
,allows ref struct
#endif
    {
        var nullable = type.IsNullableType();
        if (nullable)
        {
            type = type.GenericTypeArguments[0];
        }

        #region Null

        if (reader.ReadIsNull())
        {
            return nullable ? null : type.GetDefault();
        }

        #endregion Null

        #region Enum

        if (type.IsEnum)
        {
            var enumType = Enum.GetUnderlyingType(type);

            if (enumType == typeof(byte))
            {
                return Enum.ToObject(type, reader.ReadByte());
            }
            else if (enumType == typeof(sbyte))
            {
                return Enum.ToObject(type, reader.ReadInt16());
            }
            else if (enumType == typeof(short))
            {
                return Enum.ToObject(type, reader.ReadInt16());
            }
            else if (enumType == typeof(ushort))
            {
                return Enum.ToObject(type, reader.ReadUInt16());
            }
            else if (enumType == typeof(int))
            {
                return Enum.ToObject(type, reader.ReadInt32());
            }
            else
            {
                return enumType == typeof(uint)
                    ? Enum.ToObject(type, reader.ReadUInt32())
                    : enumType == typeof(ulong) ? Enum.ToObject(type, reader.ReadUInt64()) : Enum.ToObject(type, reader.ReadInt64());
            }
        }

        #endregion Enum

        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                return reader.ReadBoolean();

            case TypeCode.Char:
                return reader.ReadChar();

            case TypeCode.SByte:
                return (sbyte)reader.ReadInt16();

            case TypeCode.Byte:
                return reader.ReadByte();

            case TypeCode.Int16:
                return reader.ReadInt16();

            case TypeCode.UInt16:
                return reader.ReadUInt16();

            case TypeCode.Int32:
                return reader.ReadInt32();

            case TypeCode.UInt32:
                return reader.ReadUInt32();

            case TypeCode.Int64:
                return reader.ReadInt64();

            case TypeCode.UInt64:
                return reader.ReadUInt64();

            case TypeCode.Single:
                return reader.ReadFloat();

            case TypeCode.Double:
                return reader.ReadDouble();

            case TypeCode.Decimal:
                return reader.ReadDecimal();

            case TypeCode.DateTime:
                return reader.ReadDateTime();

            case TypeCode.String:
                return reader.ReadString();

            default:
                {
                    if (type == typeof(byte[]))
                    {
                        return reader.ReadBytesPackageSpan().ToArray();
                    }
                    else if (type.IsClass || type.IsStruct())
                    {
                        var len = reader.ReadInt32();
                        var serializeObj = serializerContext.GetSerializeObject(type);
                        return serializeObj.Converter != null
                            ? serializeObj.Converter.Read(ref reader, type)
                            : DeserializeClass(type, ref reader, len, serializerContext);
                    }
                    else
                    {
                        throw new Exception("未定义的类型：" + type.ToString());
                    }
                }
        }
    }

    private static object DeserializeClass<TReader>(Type type, ref TReader reader, int length, FastSerializerContext serializerContext) where TReader : IByteBlockReader
#if AllowsRefStruct
,allows ref struct
#endif
    {
        var serializeObject = serializerContext.GetSerializeObject(type);

        object instance;
        switch (serializeObject.InstanceType)
        {
            case InstanceType.Class:
                {
                    instance = serializerContext.GetNewInstance(type);

                    var index = reader.Position + length;

                    if (serializeObject.EnableIndex)
                    {
                        while (reader.Position < index)
                        {
                            var propertyNameIndex = reader.ReadByte();
                            if (serializeObject.IsStruct)
                            {
                                if (serializeObject.FastMemberInfoDicForIndex.TryGetValue(propertyNameIndex, out var property))
                                {
                                    var obj = Deserialize(property.Type, ref reader, serializerContext);
                                    property.SetValue(ref instance, obj);
                                }
                                else
                                {
                                    IgnoreLength(ref reader, type);
                                }
                            }
                            else
                            {
                                if (serializeObject.FastMemberInfoDicForIndex.TryGetValue(propertyNameIndex, out var property))
                                {
                                    var obj = Deserialize(property.Type, ref reader, serializerContext);
                                    serializeObject.MemberAccessor.SetValue(instance, property.Name, obj);
                                }
                                else
                                {
                                    IgnoreLength(ref reader, type);
                                }
                            }
                        }
                    }
                    else
                    {
                        while (reader.Position < index)
                        {
                            var propertyName = reader.ReadString(FixedHeaderType.Byte);

                            if (serializeObject.IsStruct)
                            {
                                if (serializeObject.FastMemberInfoDicForName.TryGetValue(propertyName, out var property))
                                {
                                    var obj = Deserialize(property.Type, ref reader, serializerContext);
                                    property.SetValue(ref instance, obj);
                                }
                                else
                                {
                                    IgnoreLength(ref reader, type);
                                }
                            }
                            else
                            {
                                if (serializeObject.FastMemberInfoDicForName.TryGetValue(propertyName, out var property))
                                {
                                    var obj = Deserialize(property.Type, ref reader, serializerContext);
                                    serializeObject.MemberAccessor.SetValue(instance, property.Name, obj);
                                }
                                else
                                {
                                    IgnoreLength(ref reader, type);
                                }
                            }
                        }
                    }

                    break;
                }
            case InstanceType.List:
                {
                    instance = serializerContext.GetNewInstance(type);
                    var paramLen = reader.ReadUInt32();
                    for (uint i = 0; i < paramLen; i++)
                    {
                        var obj = Deserialize(serializeObject.ArgTypes[0], ref reader, serializerContext);
                        serializeObject.AddMethod.Invoke(instance, new object[] { obj });
                    }
                    break;
                }
            case InstanceType.Array:
                {
                    var rank = serializeObject.Type.GetArrayRank();
                    if (rank == 1)
                    {
                        var paramLen = reader.ReadUInt32();
                        var array = Array.CreateInstance(serializeObject.ArgTypes[0], paramLen);

                        for (uint i = 0; i < paramLen; i++)
                        {
                            var obj = Deserialize(serializeObject.ArgTypes[0], ref reader, serializerContext);
                            array.SetValue(obj, i);
                        }
                        instance = array;
                    }
                    else
                    {
                        var rankArray = new int[rank];
                        for (var i = 0; i < rank; i++)
                        {
                            rankArray[i] = reader.ReadInt32();
                        }

                        //var paramLen = (int)byteBlock.ReadUInt32();
                        var array = Array.CreateInstance(serializeObject.ArgTypes[0], rankArray);

                        //for (int i = 0; i < rank; i++)
                        //{
                        //    var obj = Deserialize(serializeObject.ArgTypes[0], ref byteBlock, serializerContext);
                        //    array.SetValue(obj, i);
                        //}

                        var indices = new int[rank];
                        FillArrayRecursive(serializeObject, ref reader, serializerContext, array, rankArray, indices, 0);

                        instance = array;
                    }

                    break;
                }
            case InstanceType.Dictionary:
                {
                    instance = serializerContext.GetNewInstance(type);
                    var paramLen = reader.ReadUInt32();
                    for (uint i = 0; i < paramLen; i++)
                    {
                        var key = Deserialize(serializeObject.ArgTypes[0], ref reader, serializerContext);
                        var value = Deserialize(serializeObject.ArgTypes[1], ref reader, serializerContext);
                        serializeObject.AddMethod.Invoke(instance, new object[] { key, value });
                    }
                    break;
                }
            default:
                instance = null;
                break;
        }

        return instance;
    }

    private static void FillArrayRecursive<TReader>(SerializObject serializObject, ref TReader reader, FastSerializerContext serializerContext, Array array, int[] rankArray, int[] indices, int dimension)
         where TReader : IByteBlockReader
#if AllowsRefStruct
,allows ref struct
#endif
    {
        if (dimension == rankArray.Length)
        {
            // 已经到达最后一维，进行赋值操作
            var obj = Deserialize(serializObject.ArgTypes[0], ref reader, serializerContext);
            array.SetValue(obj, indices);
            return;
        }

        for (var i = 0; i < rankArray[dimension]; i++)
        {
            indices[dimension] = i;
            FillArrayRecursive(serializObject, ref reader, serializerContext, array, rankArray, indices, dimension + 1);
        }
    }

    private static void IgnoreLength<TReader>(ref TReader reader, Type type) 
        where TReader : IByteBlockReader
#if AllowsRefStruct
,allows ref struct
#endif
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Boolean:
                reader.Position+=1;
                break;

            case TypeCode.Char:
                reader.Position+=2;
                break;

            case TypeCode.SByte:
                reader.Position += 2;
                break;

            case TypeCode.Byte:
                reader.Position += 1;
                break;

            case TypeCode.Int16:
                reader.Position += 2;
                break;

            case TypeCode.UInt16:
                reader.Position += 2;
                break;

            case TypeCode.Int32:
                reader.Position += 4;
                break;

            case TypeCode.UInt32:
                reader.Position += 4;
                break;

            case TypeCode.Int64:
                reader.Position += 8;
                break;

            case TypeCode.UInt64:
                reader.Position += 8;
                break;

            case TypeCode.Single:
                reader.Position += 4;
                break;

            case TypeCode.Double:
                reader.Position += 8;
                break;

            case TypeCode.Decimal:
                reader.Position += 16;
                break;

            case TypeCode.DateTime:
                reader.Position += 8;
                break;

            case TypeCode.String:
                reader.ReadString();
                break;

            default:
                var len = reader.ReadInt32();
                reader.Position += len;
                break;
        }
    }

    #endregion Deserialize
}