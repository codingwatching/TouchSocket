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
using System.Diagnostics;
using System.Threading.Tasks;

namespace TouchSocket.Core;

/// <summary>
/// 时间测量器
/// </summary>
public class TimeMeasurer
{
    /// <summary>
    /// 开始运行
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static TimeSpan Run(Action action)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action.Invoke();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    /// <summary>
    /// 开始运行
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static TimeSpan Run(Func<Task> func)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        func.Invoke().GetFalseAwaitResult();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    /// <summary>
    /// 开始运行
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<TimeSpan> RunAsync(Func<Task> func)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await func.Invoke().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}