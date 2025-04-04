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

using Microsoft.Extensions.DependencyInjection;
using System;
using TouchSocket.Core.AspNetCore;

namespace TouchSocket.Core;

/// <summary>
/// AspNetCoreConfigExtension
/// </summary>
public static class AspNetCoreConfigExtension
{
    /// <summary>
    /// 使用<see cref="AspNetCoreContainer"/>作为容器。
    /// </summary>
    /// <param name="config"></param>
    /// <param name="services"></param>
    /// <returns></returns>
    public static TouchSocketConfig UseAspNetCoreContainer(this TouchSocketConfig config, IServiceCollection services)
    {
        config.SetRegistrator(new AspNetCoreContainer(services));
        return config;
    }

    /// <summary>
    /// 配置容器。
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureContainer(this IServiceCollection services, Action<IRegistrator> action)
    {
        var container = new AspNetCoreContainer(services);
        action.Invoke(container);
        return services;
    }
}