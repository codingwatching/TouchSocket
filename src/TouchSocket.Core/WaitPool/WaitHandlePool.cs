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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TouchSocket.Core;

/// <summary>
/// 表示一个等待句柄池。
/// </summary>
/// <typeparam name="T">等待句柄的类型。</typeparam>
public class WaitHandlePool<T> : WaitHandlePool<WaitData<T>, WaitDataAsync<T>, T>, IWaitHandlePool<T>
    where T : IWaitHandle
{
}

/// <summary>
/// 表示一个等待句柄池
/// </summary>
/// <typeparam name="TWaitData">同步等待数据</typeparam>
/// <typeparam name="TWaitDataAsync">异步等待数据</typeparam>
/// <typeparam name="T">数据体</typeparam>
public class WaitHandlePool<TWaitData, TWaitDataAsync, T> : DisposableObject, IWaitHandlePool<TWaitData, TWaitDataAsync, T>
    where TWaitData : IWaitData<T>, new()
    where TWaitDataAsync : IWaitDataAsync<T>, new()
    where T : IWaitHandle
{
    /// <summary>
    /// 不要设为readonly
    /// </summary>
    private SpinLock m_lock = new SpinLock(Debugger.IsAttached);
    private readonly Dictionary<int, TWaitData> m_waitDic = new();
    private readonly Dictionary<int, TWaitDataAsync> m_waitDicAsync = new();
    private readonly Queue<TWaitData> m_waitQueue = new();
    private readonly Queue<TWaitDataAsync> m_waitQueueAsync = new();
    private int m_currentSign;
    private int m_maxSign = int.MaxValue;
    private int m_minSign = 0;

    /// <inheritdoc/>
    public int MaxSign { get => this.m_maxSign; set => this.m_maxSign = value; }

    /// <inheritdoc/>
    public int MinSign { get => this.m_minSign; set => this.m_minSign = value; }

    /// <inheritdoc/>
    public void CancelAll()
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            foreach (var item in this.m_waitDic.Values)
            {
                item.Cancel();
            }
            foreach (var item in this.m_waitDicAsync.Values)
            {
                item.Cancel();
            }
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public void Destroy(int sign)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);

            if (this.m_waitDicAsync.TryRemove(sign, out var waitDataAsync))
            {
                if (waitDataAsync.DisposedValue)
                {
                    return;
                }

                waitDataAsync.Reset();
                this.m_waitQueueAsync.Enqueue(waitDataAsync);
                return;
            }

            if (this.m_waitDic.TryRemove(sign, out var waitData))
            {
                if (waitData.DisposedValue)
                {
                    return;
                }
                waitData.Reset();
                this.m_waitQueue.Enqueue(waitData);
            }
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    [Obsolete("此方法在调用时，可能导致不可控bug，已被弃用，请使用Destroy(in sign)的重载函数直接代替", true)]
    public void Destroy(TWaitData waitData)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            if (waitData.WaitResult == null)
            {
                return;
            }
            if (this.m_waitDic.Remove(waitData.WaitResult.Sign))
            {
                if (waitData.DisposedValue)
                {
                    return;
                }

                waitData.Reset();
                this.m_waitQueue.Enqueue(waitData);
            }
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    [Obsolete("此方法在调用时，可能导致不可控bug，已被弃用，请使用Destroy(in sign)的重载函数直接代替", true)]
    public void Destroy(TWaitDataAsync waitData)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);

            if (waitData.WaitResult == null)
            {
                return;
            }
            if (this.m_waitDicAsync.Remove(waitData.WaitResult.Sign))
            {
                if (waitData.DisposedValue)
                {
                    return;
                }

                waitData.Reset();
                this.m_waitQueueAsync.Enqueue(waitData);
            }
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public TWaitData GetWaitData(T result, bool autoSign = true)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            if (this.m_waitQueue.TryDequeue(out var waitData))
            {
                if (autoSign)
                {
                    result.Sign = this.GetSign();
                }
                waitData.SetResult(result);
                this.m_waitDic.Add(result.Sign, waitData);
                return waitData;
            }

            // 如果队列中没有可取出的等待数据对象，则新建一个
            waitData = new TWaitData();
            // 如果自动签名开启，则为结果对象设置签名
            if (autoSign)
            {
                result.Sign = this.GetSign();
            }
            // 设置等待数据对象的结果
            waitData.SetResult(result);
            // 将结果对象的签名和等待数据对象添加到字典中
            this.m_waitDic.Add(result.Sign, waitData);
            return waitData;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public TWaitData GetWaitData(out int sign)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            // 尝试从同步等待队列中取出一个等待数据对象
            if (this.m_waitQueue.TryDequeue(out var waitData))
            {
                // 生成签名
                sign = this.GetSign();
                // 设置等待数据对象的默认结果
                waitData.SetResult(default);
                // 将签名和等待数据对象添加到字典中
                this.m_waitDic.Add(sign, waitData);
                return waitData;
            }

            // 如果队列中没有可取出的等待数据对象，则新建一个
            waitData = new TWaitData();
            // 生成签名
            sign = this.GetSign();
            // 设置等待数据对象的默认结果
            waitData.SetResult(default);
            // 将签名和等待数据对象添加到字典中
            this.m_waitDic.Add(sign, waitData);
            return waitData;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public TWaitDataAsync GetWaitDataAsync(T result, bool autoSign = true)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            // 尝试从异步等待队列中取出一个等待数据对象
            if (this.m_waitQueueAsync.TryDequeue(out var waitData))
            {
                // 如果自动签名开启，则为结果对象设置签名
                if (autoSign)
                {
                    result.Sign = this.GetSign();
                }
                // 设置等待数据对象的结果
                waitData.SetResult(result);
                // 将结果对象的签名和等待数据对象添加到字典中
                this.m_waitDicAsync.Add(result.Sign, waitData);
                return waitData;
            }

            // 如果队列中没有可取出的等待数据对象，则新建一个
            waitData = new TWaitDataAsync();
            // 如果自动签名开启，则为结果对象设置签名
            if (autoSign)
            {
                result.Sign = this.GetSign();
            }
            // 设置等待数据对象的结果
            waitData.SetResult(result);
            // 将结果对象的签名和等待数据对象添加到字典中
            this.m_waitDicAsync.Add(result.Sign, waitData);
            return waitData;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public TWaitDataAsync GetWaitDataAsync(out int sign)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            // 尝试从异步等待队列中取出一个等待数据对象
            if (this.m_waitQueueAsync.TryDequeue(out var waitData))
            {
                // 生成签名
                sign = this.GetSign();
                // 设置等待数据对象的默认结果
                waitData.SetResult(default);
                // 将签名和等待数据对象添加到字典中
                this.m_waitDicAsync.Add(sign, waitData);
                return waitData;
            }

            // 如果队列中没有可取出的等待数据对象，则新建一个
            waitData = new TWaitDataAsync();
            // 生成签名
            sign = this.GetSign();
            // 设置等待数据对象的默认结果
            waitData.SetResult(default);
            // 将签名和等待数据对象添加到字典中
            this.m_waitDicAsync.Add(sign, waitData);
            return waitData;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public bool SetRun(int sign)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            // 尝试从异步等待数据字典中获取并设置等待数据
            if (this.m_waitDicAsync.TryGetValue(sign, out var waitDataAsync))
            {
                waitDataAsync.Set();
                return true;
            }

            // 尝试从同步等待数据字典中获取并设置等待数据
            if (this.m_waitDic.TryGetValue(sign, out var waitData))
            {
                waitData.Set();
                return true;
            }

            return false;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public bool SetRun(int sign, T waitResult)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            // 尝试从异步等待数据字典中获取并设置等待数据
            if (this.m_waitDicAsync.TryGetValue(sign, out var waitDataAsync))
            {
                waitDataAsync.Set(waitResult);
                return true;
            }
            // 尝试从同步等待数据字典中获取并设置等待数据
            if (this.m_waitDic.TryGetValue(sign, out var waitData))
            {
                waitData.Set(waitResult);
                return true;
            }

            return false;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public bool SetRun(T waitResult)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            // 尝试从异步等待数据字典中获取并设置等待数据
            if (this.m_waitDicAsync.TryGetValue(waitResult.Sign, out var waitDataAsync))
            {
                waitDataAsync.Set(waitResult);
                return true;
            }

            // 尝试从同步等待数据字典中获取并设置等待数据
            if (this.m_waitDic.TryGetValue(waitResult.Sign, out var waitData))
            {
                waitData.Set(waitResult);
                return true;
            }

            return false;
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public bool TryGetData(int sign, out TWaitData waitData)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            return this.m_waitDic.TryGetValue(sign, out waitData);
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    public bool TryGetDataAsync(int sign, out TWaitDataAsync waitDataAsync)
    {
        var lockTaken = false;
        try
        {
            this.m_lock.Enter(ref lockTaken);
            return this.m_waitDicAsync.TryGetValue(sign, out waitDataAsync);
        }
        finally
        {
            if (lockTaken)
            {
                this.m_lock.Exit(false);
            }
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            var lockTaken = false;
            try
            {
                this.m_lock.Enter(ref lockTaken);
                foreach (var item in this.m_waitDic.Values)
                {
                    item.SafeDispose();
                }
                foreach (var item in this.m_waitQueue)
                {
                    item.SafeDispose();
                }
                this.m_waitDic.Clear();

                this.m_waitQueue.Clear();
            }
            finally
            {
                if (lockTaken)
                {
                    this.m_lock.Exit(false);
                }
            }
        }

        base.Dispose(disposing);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetSign()
    {
        var sign = this.m_currentSign++;
        if (this.m_currentSign >= this.m_maxSign)
        {
            this.m_currentSign = this.m_minSign;
        }
        return sign;
    }
}
