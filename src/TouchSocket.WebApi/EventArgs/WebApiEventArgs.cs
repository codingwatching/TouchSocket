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
using TouchSocket.Http;

namespace TouchSocket.WebApi;

/// <summary>
/// 提供Web API事件参数的类
/// </summary>
public partial class WebApiEventArgs : PluginEventArgs
{
    /// <summary>
    /// 初始化WebApiEventArgs类的新实例
    /// </summary>
    /// <param name="request">表示HTTP请求的对象</param>
    /// <param name="response">表示HTTP响应的对象</param>
    public WebApiEventArgs(HttpRequest request, HttpResponse response)
    {
        this.Request = request;
        this.Response = response;
        this.IsHttpMessage = false;
    }

    /// <summary>
    /// 获取或设置一个值，该值指示是否以HttpMessage方式请求
    /// </summary>
    public bool IsHttpMessage { get; set; }

    /// <summary>
    /// 获取HTTP请求对象
    /// </summary>
    public HttpRequest Request { get; }

    /// <summary>
    /// 获取HTTP响应对象
    /// </summary>
    public HttpResponse Response { get; }
}

#if NETSTANDARD2_0_OR_GREATER || NET481_OR_GREATER || NET6_0_OR_GREATER

/// <summary>
/// WebAPI事件参数类，用于封装HTTP请求和响应信息
/// </summary>
public partial class WebApiEventArgs
{
    /// <summary>
    /// Http请求
    /// </summary>
    public System.Net.Http.HttpRequestMessage RequestMessage { get; }

    /// <summary>
    /// WebApiEventArgs
    /// </summary>
    /// <param name="requestMessage">HTTP请求消息</param>
    /// <param name="responseMessage">HTTP响应消息</param>
    public WebApiEventArgs(System.Net.Http.HttpRequestMessage requestMessage, System.Net.Http.HttpResponseMessage responseMessage)
    {
        this.RequestMessage = requestMessage;
        this.ResponseMessage = responseMessage;
        this.IsHttpMessage = true;
    }

    /// <summary>
    /// Http响应
    /// </summary>
    public System.Net.Http.HttpResponseMessage ResponseMessage { get; }
}

#endif