using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Vault.BetterCoroutine {
    /// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
    /// It is an error to attempt to start a task that has been stopped or which has
    /// naturally terminated.
    public class AsyncRuntime : IAsyncRuntime {
        private bool _isFinished;

        public bool IsFinished => _isFinished;

        private readonly TaskManager.TaskState _task;

        private Action _afterFinished;

        private readonly Task _currentTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();


        private AsyncRuntime(UniTask currentTask, CancellationTokenSource cancellationTokenSource) : this(currentTask) {
            _cancellationTokenSource = cancellationTokenSource;
        }

        private AsyncRuntime(UniTask currentTask) {
            currentTask.AttachExternalCancellation(_cancellationTokenSource.Token);
            _currentTask = currentTask.AsTask();
        }

        private AsyncRuntime(Action toExecute, bool isEndOfFrame) {
            _currentTask = isEndOfFrame ? WaitForEndOfFrameInternal(toExecute) : CreateInternal(toExecute);
            _currentTask.ContinueWith(task => {
                if (task.IsFaulted && task.Exception != null) {
                    UnityThread.executeInUpdate(() => throw task.Exception);
                }
            });
            // _currentTask.GetAwaiter()
            //     .OnCompleted(() => {
            //         if (_currentTask.Exception == null) return;
            //         foreach (var exceptionDispatchInfo in _currentTask.Exception.InnerExceptions
            //             .Select(ExceptionDispatchInfo.Capture)) {
            //             exceptionDispatchInfo.Throw();
            //         }
            //     });
        }

        private AsyncRuntime(Action toExecute, Func<bool> waitUntilToExecute) {
            _currentTask = WaitUntilInternal(waitUntilToExecute, toExecute);
            _currentTask.ContinueWith(task => {
                if (task.IsFaulted && task.Exception != null) {
                    UnityThread.executeInUpdate(() => throw task.Exception);
                }
            });
            // _currentTask.GetAwaiter()
            //     .OnCompleted(() => {
            //         if (_currentTask.Exception == null) return;
            //         foreach (var exceptionDispatchInfo in _currentTask.Exception.InnerExceptions
            //             .Select(ExceptionDispatchInfo.Capture)) {
            //             exceptionDispatchInfo.Throw();
            //         }
            //     });
        }

        private AsyncRuntime(Action toExecute, float secondsUntilExecute) {
            _currentTask = WaitForSecondsInternal(toExecute, secondsUntilExecute);
            _currentTask.ContinueWith(task => {
                if (task.IsFaulted && task.Exception != null) {
                    UnityThread.executeInUpdate(() => throw task.Exception);
                }
            });
            // _currentTask.GetAwaiter()
            //     .OnCompleted(() => {
            //         if (_currentTask.Exception == null) return;
            //         foreach (var exceptionDispatchInfo in _currentTask.Exception.InnerExceptions
            //             .Select(ExceptionDispatchInfo.Capture)) {
            //             exceptionDispatchInfo.Throw();
            //         }
            //     });
        }

        private AsyncRuntime(Action todo, Func<bool> toAbort, Func<float> seconds) {
            _currentTask = EverySecondDoInternal(todo, toAbort, seconds);
            _currentTask.ContinueWith(task => {
                if (task.IsFaulted && task.Exception != null) {
                    UnityThread.executeInUpdate(() => throw task.Exception);
                }
            });
            // _currentTask.GetAwaiter()
            //     .OnCompleted(() => {
            //         if (_currentTask.Exception == null) return;
            //         foreach (var exceptionDispatchInfo in _currentTask.Exception.InnerExceptions
            //             .Select(ExceptionDispatchInfo.Capture)) {
            //             exceptionDispatchInfo.Throw();
            //         }
            //     });
        }


        /// Returns true if and only if the coroutine is running.  Paused tasks
        /// are considered to be running.
        public bool Running => _currentTask.Status.Equals(TaskStatus.Running) ||
                               _currentTask.Status.Equals(TaskStatus.WaitingForActivation);

        /// Returns true if and only if the coroutine is currently paused.
        public bool Paused => _task.Paused;

        public void Wait() {
            _currentTask.Wait();
        }

        private Task CreateInternal(Action action) {
            var task = new Task(() => {
                    action.Invoke();
                    TaskFinished(false);
                },
                _cancellationTokenSource.Token);
            return task;
        }

        private async Task WaitUntilInternal(Func<bool> trueBefore, Action toExecute) {
            await UniTask.WaitUntil(trueBefore, cancellationToken: _cancellationTokenSource.Token);
            toExecute?.Invoke();
            TaskFinished(false);
        }

        private async Task WaitForSecondsInternal(Action action, float seconds) {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds),
                DelayType.Realtime,
                cancellationToken: _cancellationTokenSource.Token);
            action?.Invoke();
            TaskFinished(false);
        }

        private async Task WaitForEndOfFrameInternal(Action action) {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            action?.Invoke();
            TaskFinished(false);
        }

        private async Task EverySecondDoInternal(Action todo, Func<bool> toAbort, Func<float> seconds) {
            while (!toAbort?.Invoke() ?? true) {
                await UniTask.Delay(TimeSpan.FromSeconds(seconds.Invoke()),
                    cancellationToken: _cancellationTokenSource.Token);
                todo.Invoke();
            }

            TaskFinished(false);
        }

        public IAsyncRuntime AndAfter(Action action) {
            var asyncRuntime = new AsyncRuntime(action, false);
            _currentTask.GetAwaiter().OnCompleted(() => asyncRuntime.Start());
            return asyncRuntime;
        }

        public IAsyncRuntime AndWaitUntil(Func<bool> isTrue, Action toExecute) {
            var andAfterWaitUntil = new AsyncRuntime(toExecute, isTrue);
            _currentTask.GetAwaiter().OnCompleted(() => andAfterWaitUntil.Start());
            return andAfterWaitUntil;
        }

        public IAsyncRuntime AndAfterSeconds(Action action, float seconds) {
            var andAfterWaitForSeconds = new AsyncRuntime(action, seconds);
            _currentTask.GetAwaiter().OnCompleted(() => andAfterWaitForSeconds.Start());
            return andAfterWaitForSeconds;
        }


        /// Termination event.  Triggered when the coroutine completes execution.
        public event IAsyncRuntime.FinishedHandler OnFinished;

        /// Begins execution of the coroutine
        public void Start() {
            _currentTask.Start();
        }

        /// Discontinues execution of the coroutine at its next yield.
        public void Stop() {
            _cancellationTokenSource.Cancel();
            _currentTask.Dispose();
            OnFinished?.Invoke(true);
        }

        public void Pause() {
            _task.Pause();
        }

        public void Unpause() {
            _task.Unpause();
        }


        public void AndAfterFinishDo(Action afterFinished) {
            _afterFinished = afterFinished;
        }

        private void TaskFinished(bool manual) {
            _isFinished = true;
            var handler = OnFinished;
            UnityThread.executeInUpdate(() => _afterFinished?.Invoke());
            handler?.Invoke(manual);
        }


        public static IAsyncRuntime Create(UniTask task) {
            return new AsyncRuntime(task);
        }

        public static IAsyncRuntime Create(UniTask task, CancellationTokenSource cancellationTokenSource) {
            return new AsyncRuntime(task, cancellationTokenSource);
        }
        

        public static IAsyncRuntime Create(Action action) {
            return new AsyncRuntime(action, false);
        }

        public static IAsyncRuntime WaitUntil(Func<bool> isTrue, Action toExecute) {
            return new AsyncRuntime(toExecute, isTrue);
        }

        public static IAsyncRuntime WaitForSeconds(Action action, float seconds) {
            return new AsyncRuntime(action, seconds);
        }

        public static IAsyncRuntime WaitForEndOfFrame(Action action, bool autostart = true) {
            return new AsyncRuntime(action, true);
        }

        public static IAsyncRuntime EverySecondsDo(Action todo,
            Func<float> seconds,
            Func<bool> toAbort = null,
            bool autostart = true) {
            return new AsyncRuntime(todo, toAbort, seconds);
        }
    }
}