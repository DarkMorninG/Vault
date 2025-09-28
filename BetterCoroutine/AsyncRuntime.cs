using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Vault.BetterCoroutine {
    /// A Task object represents a coroutine. Tasks can be started, paused, and stopped.
    /// Converted to use UniTask only, with robust main-thread exception handling.
    public class AsyncRuntime : IAsyncRuntime {
        private bool _isFinished;
        private bool _isRunning;
        private bool _isPaused;
        private UniTaskCompletionSource<bool> _resumeTcs;

        public bool IsFinished => _isFinished;

        private Action _afterFinished;

        private UniTask _currentTask;
        private CancellationTokenSource _cancellationTokenSource = new();

        static AsyncRuntime() {
            UniTaskScheduler.UnobservedTaskException += UnityEngine.Debug.Log;
        }


        private AsyncRuntime(UniTask currentTask, CancellationTokenSource cancellationTokenSource) : this(currentTask) {
            _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
            _currentTask = WrapWithPause(currentTask).AttachExternalCancellation(_cancellationTokenSource.Token);
        }

        private AsyncRuntime(UniTask currentTask) {
            _currentTask = WrapWithPause(currentTask).AttachExternalCancellation(_cancellationTokenSource.Token);
        }

        private AsyncRuntime(Action toExecute, bool isEndOfFrame) {
            _currentTask = isEndOfFrame ? WaitForEndOfFrameInternal(toExecute) : CreateInternal(toExecute);
        }

        private AsyncRuntime(Action toExecute, Func<bool> waitUntilToExecute) {
            _currentTask = WaitUntilInternal(waitUntilToExecute, toExecute);
        }

        private AsyncRuntime(Action toExecute, float secondsUntilExecute) {
            _currentTask = WaitForSecondsInternal(toExecute, secondsUntilExecute);
        }

        private AsyncRuntime(Action todo, Func<bool> toAbort, Func<float> seconds) {
            _currentTask = EverySecondDoInternal(todo, toAbort, seconds);
        }

        /// Returns true if and only if the coroutine is running. Paused tasks are considered to be running.
        public bool Running => _isRunning && !_isFinished;

        /// Returns true if and only if the coroutine is currently paused.
        public bool Paused => _isPaused;

        public void Wait() {
            // Synchronously waits for the UniTask to complete. Avoid on main thread.
            _currentTask.GetAwaiter().GetResult();
        }

        private async UniTask WaitWhilePaused() {
            // Simple pause gate: waits until unpaused; releases also when stopped.
            while (_isPaused && !_isFinished) {
                if (_cancellationTokenSource.IsCancellationRequested) break;
                _resumeTcs ??= new UniTaskCompletionSource<bool>();
                await _resumeTcs.Task;
            }
        }

        private async UniTask WrapWithPause(UniTask task) {
            await WaitWhilePaused();
            await task;
        }

        private async UniTask CreateInternal(Action action) {
            try {
                await WaitWhilePaused();
                await UniTask.RunOnThreadPool(() => action?.Invoke());
            }
            finally {
                TaskFinished(false);
            }
        }

        private async UniTask WaitUntilInternal(Func<bool> trueBefore, Action toExecute) {
            await WaitWhilePaused();
            await UniTask.WaitUntil(trueBefore, cancellationToken: _cancellationTokenSource.Token);
            await WaitWhilePaused();
            toExecute?.Invoke();
            TaskFinished(false);
        }

        private async UniTask WaitForSecondsInternal(Action action, float seconds) {
            await WaitWhilePaused();
            await UniTask.Delay(TimeSpan.FromSeconds(seconds), DelayType.Realtime, cancellationToken: _cancellationTokenSource.Token);
            await WaitWhilePaused();
            action?.Invoke();
            TaskFinished(false);
        }

        private async UniTask WaitForEndOfFrameInternal(Action action) {
            await WaitWhilePaused();
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            await WaitWhilePaused();
            action?.Invoke();
            TaskFinished(false);
        }

        private async UniTask EverySecondDoInternal(Action todo, Func<bool> toAbort, Func<float> seconds) {
            try {
                while (!toAbort?.Invoke() ?? true) {
                    await WaitWhilePaused();
                    var waitSeconds = seconds != null ? seconds.Invoke() : 1f;
                    await UniTask.Delay(TimeSpan.FromSeconds(waitSeconds), cancellationToken: _cancellationTokenSource.Token);
                    await WaitWhilePaused();
                    todo?.Invoke();
                }
            }
            finally {
                TaskFinished(false);
            }
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

        /// Termination event. Triggered when the coroutine completes execution.
        public event IAsyncRuntime.FinishedHandler OnFinished;

        /// Begins execution of the coroutine
        public void Start() {
            if (_isRunning || _isFinished) return;
            _isRunning = true;
            Run().Forget();
        }

        private async UniTask Run() {
            try {
                await _currentTask.AttachExternalCancellation(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) {
                // Cancellation is considered manual stop
                await UniTask.SwitchToMainThread();
                OnFinished?.Invoke(true);
            }
            catch (Exception ex) {
                // Ensure exceptions are logged on Unity main thread
                await UniTask.SwitchToMainThread();
                UnityEngine.Debug.LogError(ex);
                TaskFinished(false);
            }
            finally {
                _isRunning = false;
                // If no internal flow signaled finish yet, do it now
                if (!_isFinished) TaskFinished(false);
            }
        }

        /// Discontinues execution of the coroutine at its next yield.
        public void Stop() {
            if (_isFinished) return;
            // ensure any paused waiters are released
            Unpause();
            _cancellationTokenSource.Cancel();
            _isRunning = false;
            OnFinished?.Invoke(true);
        }

        public void Pause() {
            if (_isFinished) return;
            _isPaused = true;
            // create a new gate if none exists, so awaiters can wait
            _resumeTcs ??= new UniTaskCompletionSource<bool>();
        }

        public void Unpause() {
            if (!_isPaused) return;
            _isPaused = false;
            // release any awaiters
            _resumeTcs?.TrySetResult(true);
            _resumeTcs = null;
        }

        public void AndAfterFinishDo(Action afterFinished) {
            _afterFinished = afterFinished;
        }

        private void TaskFinished(bool manual) {
            if (_isFinished) return;
            _isFinished = true;
            var handler = OnFinished;
            // Invoke callbacks on main thread
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