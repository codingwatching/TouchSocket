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
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace TouchSocket.JsonRpc;

/// <summary>
/// JsonRpcClientExtension
/// </summary>
public static class JsonRpcClientExtension
{
    /// <summary>
    /// JsonRpcActorProperty
    /// </summary>
    public static readonly DependencyProperty<JsonRpcActor> JsonRpcActorProperty =
       new("JsonRpcActor", default);

    /// <summary>
    /// 获取基于会话内部的双工JsonRpc端
    /// </summary>
    /// <param name="sessionClient"></param>
    /// <returns></returns>
    public static IJsonRpcClient GetJsonRpcActionClient(this ISessionClient sessionClient)
    {
        if (sessionClient.TryGetValue(JsonRpcActorProperty, out var actionClient))
        {
            return actionClient;
        }
        else
        {
            throw new System.Exception("SessionClient必须是Tcp协议，或者完成WebSocket连接");
        }
    }

#if SystemTextJson

    /// <summary>
    /// 使用System.Text.Json进行序列化
    /// </summary>
    /// <param name="jsonRpcClient"></param>
    /// <param name="options"></param>
    public static TJsonRpcClient UseSystemTextJson<TJsonRpcClient>(this TJsonRpcClient jsonRpcClient, Action<System.Text.Json.JsonSerializerOptions> options)
        where TJsonRpcClient : IJsonRpcClient
    {
        var serializerOptions = new System.Text.Json.JsonSerializerOptions();
        options.Invoke(serializerOptions);
        jsonRpcClient.SerializerConverter.Clear();
        jsonRpcClient.SerializerConverter.Add(new SystemTextJsonStringToClassSerializerFormatter<JsonRpcActor>() { JsonSettings = serializerOptions });

        return jsonRpcClient;
    }
#endif
}