////------------------------------------------------------------------------------
////  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
////  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
////  CSDN博客：https://blog.csdn.net/qq_40374647
////  哔哩哔哩视频：https://space.bilibili.com/94253567
////  Gitee源代码仓库：https://gitee.com/RRQM_Home
////  Github源代码仓库：https://github.com/RRQM
////  API首页：https://touchsocket.net/
////  交流QQ群：234762506
////  感谢您的下载和使用
////------------------------------------------------------------------------------

//#if !(NET481_OR_GREATER || NET6_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)

//using System;
//using System.Diagnostics;
//using System.Runtime.ExceptionServices;
//using System.Runtime.InteropServices;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Threading.Tasks.Sources;

//namespace TouchSocket.Core;

///// <summary>Provides the core logic for implementing a manual-reset <see cref="IValueTaskSource"/> or <see cref="IValueTaskSource{TResult}"/>.</summary>
///// <typeparam name="TResult"></typeparam>
//[StructLayout(LayoutKind.Auto)]
//public struct ManualResetValueTaskSourceCore<TResult>
//{
//    /// <summary>
//    /// The callback to invoke when the operation completes if <see cref="OnCompleted"/> was called before the operation completed,
//    /// or <see cref="ManualResetValueTaskSourceCoreShared.s_sentinel"/> if the operation completed before a callback was supplied,
//    /// or null if a callback hasn't yet been provided and the operation hasn't yet completed.
//    /// </summary>
//    private Action<object> _continuation;
//    /// <summary>State to pass to <see cref="_continuation"/>.</summary>
//    private object _continuationState;
//    /// <summary><see cref="ExecutionContext"/> to flow to the callback, or null if no flowing is required.</summary>
//    private ExecutionContext _executionContext;
//    /// <summary>
//    /// A "captured" <see cref="SynchronizationContext"/> or <see cref="TaskScheduler"/> with which to invoke the callback,
//    /// or null if no special context is required.
//    /// </summary>
//    private object _capturedContext;
//    /// <summary>Whether the current operation has completed.</summary>
//    private bool _completed;
//    /// <summary>The result with which the operation succeeded, or the default value if it hasn't yet completed or failed.</summary>
//    private TResult _result;
//    /// <summary>The exception with which the operation failed, or null if it hasn't yet completed or completed successfully.</summary>
//    private ExceptionDispatchInfo _error;
//    /// <summary>The current version of this value, used to help prevent misuse.</summary>
//    private short _version;

//    /// <summary>Gets or sets whether to force continuations to run asynchronously.</summary>
//    /// <remarks>Continuations may run asynchronously if this is false, but they'll never run synchronously if this is true.</remarks>
//    public bool RunContinuationsAsynchronously { get; set; }

//    /// <summary>Resets to prepare for the next operation.</summary>
//    public void Reset()
//    {
//        // Reset/update state for the next use/await of this instance.
//        this._version++;
//        this._completed = false;
//        this._result = default;
//        this._error = null;
//        this._executionContext = null;
//        this._capturedContext = null;
//        this._continuation = null;
//        this._continuationState = null;
//    }

//    /// <summary>Completes with a successful result.</summary>
//    /// <param name="result">The result.</param>
//    public void SetResult(TResult result)
//    {
//        this._result = result;
//        this.SignalCompletion();
//    }

//    /// <summary>Complets with an error.</summary>
//    /// <param name="error"></param>
//    public void SetException(Exception error)
//    {
//        this._error = ExceptionDispatchInfo.Capture(error);
//        this.SignalCompletion();
//    }

//    /// <summary>Gets the operation version.</summary>
//    public short Version => this._version;

//    /// <summary>Gets the status of the operation.</summary>
//    /// <param name="token">Opaque value that was provided to the <see cref="ValueTask"/>'s constructor.</param>
//    public ValueTaskSourceStatus GetStatus(short token)
//    {
//        this.ValidateToken(token);
//        return
//            this._continuation == null || !this._completed ? ValueTaskSourceStatus.Pending :
//            this._error == null ? ValueTaskSourceStatus.Succeeded :
//            this._error.SourceException is OperationCanceledException ? ValueTaskSourceStatus.Canceled :
//            ValueTaskSourceStatus.Faulted;
//    }

//    /// <summary>Gets the result of the operation.</summary>
//    /// <param name="token">Opaque value that was provided to the <see cref="ValueTask"/>'s constructor.</param>
//    public TResult GetResult(short token)
//    {
//        this.ValidateToken(token);
//        if (!this._completed)
//        {
//            throw new InvalidOperationException();
//        }

//        this._error?.Throw();
//        return this._result;
//    }

//    /// <summary>Schedules the continuation action for this operation.</summary>
//    /// <param name="continuation">The continuation to invoke when the operation has completed.</param>
//    /// <param name="state">The state object to pass to <paramref name="continuation"/> when it's invoked.</param>
//    /// <param name="token">Opaque value that was provided to the <see cref="ValueTask"/>'s constructor.</param>
//    /// <param name="flags">The flags describing the behavior of the continuation.</param>
//    public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
//    {
//        if (continuation is null)
//        {
//            throw new ArgumentNullException(nameof(continuation));
//        }
//        this.ValidateToken(token);

//        if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
//        {
//            this._executionContext = ExecutionContext.Capture();
//        }

//        if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
//        {
//            var sc = SynchronizationContext.Current;
//            if (sc != null && sc.GetType() != typeof(SynchronizationContext))
//            {
//                this._capturedContext = sc;
//            }
//            else
//            {
//                var ts = TaskScheduler.Current;
//                if (ts != TaskScheduler.Default)
//                {
//                    this._capturedContext = ts;
//                }
//            }
//        }

//        // We need to set the continuation state before we swap in the delegate, so that
//        // if there's a race between this and SetResult/Exception and SetResult/Exception
//        // sees the _continuation as non-null, it'll be able to invoke it with the state
//        // stored here.  However, this also means that if this is used incorrectly (e.g.
//        // awaited twice concurrently), _continuationState might get erroneously overwritten.
//        // To minimize the chances of that, we check preemptively whether _continuation
//        // is already set to something other than the completion sentinel.

//        object oldContinuation = this._continuation;
//        if (oldContinuation == null)
//        {
//            this._continuationState = state;
//            oldContinuation = Interlocked.CompareExchange(ref this._continuation, continuation, null);
//        }

//        if (oldContinuation != null)
//        {
//            // Operation already completed, so we need to queue the supplied callback.
//            if (!ReferenceEquals(oldContinuation, ManualResetValueTaskSourceCoreShared.s_sentinel))
//            {
//                throw new InvalidOperationException();
//            }

//            switch (this._capturedContext)
//            {
//                case null:
//                    Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
//                    break;

//                case SynchronizationContext sc:
//                    sc.Post(s =>
//                    {
//                        var tuple = (Tuple<Action<object>, object>)s;
//                        tuple.Item1(tuple.Item2);
//                    }, Tuple.Create(continuation, state));
//                    break;

//                case TaskScheduler ts:
//                    Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
//                    break;
//            }
//        }
//    }

//    /// <summary>Ensures that the specified token matches the current version.</summary>
//    /// <param name="token">The token supplied by <see cref="ValueTask"/>.</param>
//    private void ValidateToken(short token)
//    {
//        if (token != this._version)
//        {
//            throw new InvalidOperationException();
//        }
//    }

//    /// <summary>Signals that the operation has completed.  Invoked after the result or error has been set.</summary>
//    private void SignalCompletion()
//    {
//        if (this._completed)
//        {
//            throw new InvalidOperationException();
//        }
//        this._completed = true;

//        if (this._continuation != null || Interlocked.CompareExchange(ref this._continuation, ManualResetValueTaskSourceCoreShared.s_sentinel, null) != null)
//        {
//            if (this._executionContext != null)
//            {
//                ExecutionContext.Run(
//                    this._executionContext,
//                    s => ((ManualResetValueTaskSourceCore<TResult>)s).InvokeContinuation(),
//                    this);
//            }
//            else
//            {
//                this.InvokeContinuation();
//            }
//        }
//    }

//    /// <summary>
//    /// Invokes the continuation with the appropriate captured context / scheduler.
//    /// This assumes that if <see cref="_executionContext"/> is not null we're already
//    /// running within that <see cref="ExecutionContext"/>.
//    /// </summary>
//    private void InvokeContinuation()
//    {
//        Debug.Assert(this._continuation != null);

//        switch (this._capturedContext)
//        {
//            case null:
//                if (this.RunContinuationsAsynchronously)
//                {
//                    Task.Factory.StartNew(this._continuation, this._continuationState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
//                }
//                else
//                {
//                    this._continuation(this._continuationState);
//                }
//                break;

//            case SynchronizationContext sc:
//                sc.Post(s =>
//                {
//                    var state = (Tuple<Action<object>, object>)s;
//                    state.Item1(state.Item2);
//                }, Tuple.Create(this._continuation, this._continuationState));
//                break;

//            case TaskScheduler ts:
//                Task.Factory.StartNew(this._continuation, this._continuationState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, ts);
//                break;
//        }
//    }
//}

//internal static class ManualResetValueTaskSourceCoreShared // separated out of generic to avoid unnecessary duplication
//{
//    internal static readonly Action<object> s_sentinel = CompletionSentinel;
//    private static void CompletionSentinel(object _) // named method to aid debugging
//    {
//        Debug.Fail("The sentinel delegate should never be invoked.");
//        throw new InvalidOperationException();
//    }
//}
//#endif