using System;
using System.Collections;

namespace Vault.BetterCoroutine {
    public interface IAsyncRuntimeTyped {
        /// Returns true if and only if the coroutine is running.  Paused tasks
        /// are considered to be running.
        bool Running { get; }

        /// Returns true if and only if the coroutine is currently paused.
        bool Paused { get; }

        bool IsFinished { get; }

        /// Termination event.  Triggered when the coroutine completes execution.
        event IAsyncRuntime.FinishedHandler OnFinished;

        /// Begins execution of the coroutine
        void Start();

        /// Discontinues execution of the coroutine at its next yield.
        void Stop();

        void Pause();
        void Unpause();
        void AndAfterFinishDo(Action afterFinished);
        IAsyncRuntime AndAfter(Action c);
        IAsyncRuntime AndWaitUntil(Func<bool> isTrue, Action toExecute);
        IAsyncRuntime AndAfterSeconds(Action action, float seconds);
    }
}