using System;
using System.Collections;
using UnityEngine;

namespace Vault.BetterCoroutine {
    public class UnityAsyncRuntime : IAsyncRuntime {
        /// Delegate for termination subscribers.  manual is true if and only if
        /// the coroutine was stopped with an explicit call to Stop().
        private readonly TaskManager.TaskState _task;

        private Action _afterFinish;

        /// Creates a new Task object for the given coroutine.
        /// 
        /// If autoStart is true (default) the task is automatically startedffff
        /// upon construction.
        public UnityAsyncRuntime(IEnumerator c, bool autoStart = true) {
            _task = TaskManager.CreateTask(c);
            _task.OnFinished += TaskFinished;
            if (autoStart)
                Start();
        }

        /// Returns true if and only if the coroutine is running.  Paused tasks
        /// are considered to be running.
        public bool Running => _task.Running;

        /// Returns true if and only if the coroutine is currently paused.
        public bool Paused => _task.Paused;

        public bool IsFinished => !_task.Running && !_task.Paused;


        private static IEnumerator WaitUntilInternal(Func<bool> trueBefore, Action toExecute) {
            yield return new WaitUntil(trueBefore);
            toExecute?.Invoke();
        }

        private static IEnumerator CreateInternal(Action toExecute) {
            yield return new WaitForEndOfFrame();
            toExecute.Invoke();
        }

        private static IEnumerator WaitForSecondsInternal(Action action, float seconds) {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        private static IEnumerator WaitForEndOfFrameInternal(Action action) {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        private static IEnumerator EverySecondsDoInternal(Action todo, Func<bool> toAbort, Func<float> seconds) {
            while (toAbort?.Invoke() ?? true) {
                yield return new WaitForSeconds(seconds.Invoke());
                todo?.Invoke();
            }
        }

        /// Termination event.  Triggered when the coroutine completes execution.
        public event IAsyncRuntime.FinishedHandler OnFinished;

        /// Begins execution of the coroutine
        public void Start() {
            _task.Start();
        }

        /// Discontinues execution of the coroutine at its next yield.
        public void Stop() {
            _task.Stop();
        }

        public void Pause() {
            _task.Pause();
        }

        public void Unpause() {
            _task.Unpause();
        }

        public static IAsyncRuntime Create(IEnumerator c, bool autoStart = true) {
            return new UnityAsyncRuntime(c, autoStart);
        }

        public static IAsyncRuntime Create(Action c, bool autoStart = true) {
            return new UnityAsyncRuntime(CreateInternal(c), autoStart);
        }

        public static IAsyncRuntime WaitForSeconds(Action action, float seconds, bool autoStart = true) {
            return new UnityAsyncRuntime(WaitForSecondsInternal(action, seconds), autoStart);
        }

        public static IAsyncRuntime WaitUntil(Func<bool> isTrue, Action toExecute, bool autoStart = true) {
            return new UnityAsyncRuntime(WaitUntilInternal(isTrue, toExecute), autoStart);
        }

        public static IAsyncRuntime WaitForEndOfFrame(Action action, bool autoStart = true) {
            return new UnityAsyncRuntime(WaitForEndOfFrameInternal(action), autoStart);
        }

        public static IAsyncRuntime EverySecondsDo(Action todo,
            Func<float> seconds,
            Func<bool> toAbort = null,
            bool autoStart = true) {
            return new UnityAsyncRuntime(EverySecondsDoInternal(todo, toAbort, seconds), autoStart);
        }

        public void AndAfterFinishDo(Action afterFinished) {
            _afterFinish = afterFinished;
        }

        public void Wait() {
            throw new NotImplementedException();
        }

        public IAsyncRuntime AndAfter(Action c) {
            var asyncRuntime = Create(c, false);
            OnFinished += manual => asyncRuntime.Start();
            return asyncRuntime;
        }

        public IAsyncRuntime AndWaitUntil(Func<bool> isTrue, Action toExecute) {
            var asyncRuntime = WaitUntil(isTrue, toExecute, false);
            OnFinished += manual => asyncRuntime.Start();
            return asyncRuntime;
        }

        public IAsyncRuntime AndAfterSeconds(Action action, float seconds) {
            var asyncRuntime = WaitForSeconds(action, seconds);
            OnFinished += manual => asyncRuntime.Start();
            return asyncRuntime;
        }

        private void TaskFinished(bool manual) {
            _afterFinish?.Invoke();
            OnFinished?.Invoke(manual);
        }
    }
}