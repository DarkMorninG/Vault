using System;

namespace BetterCoroutine.AwaitRuntime {
    public interface IAwaitRuntime {
        delegate void WaitAction();

        bool Running { get; }
        bool IsFinished { get; }

        /// Begins execution of the coroutine
        void Start();

        /// Discontinues execution of the coroutine at its next yield.
        void Stop();

        void AndAfterFinishDo(WaitAction afterFinished);

        IAwaitRuntime AndWaitUntil(WaitAction toExecute, WaitUntilAwaitRuntime.WaitCondition isTrue) {
            var waitUntilAwaitRuntime = new WaitUntilAwaitRuntime(toExecute, isTrue, false);
            AndAfterFinishDo(() => waitUntilAwaitRuntime.Start());
            return waitUntilAwaitRuntime;
        }

        IAwaitRuntime AndAfterSeconds(WaitAction action, float seconds) {
            var waitForSecondsAwaitRuntime = new WaitForSecondsAwaitRuntime(action, seconds, false);
            AndAfterFinishDo(() => waitForSecondsAwaitRuntime.Start());
            return waitForSecondsAwaitRuntime;
        }

        IAwaitRuntime AndAfterInBackground(WaitAction action) {
            var andAfterInBackground = new BackgroundAwaitRuntime(action, false);
            AndAfterFinishDo(() => andAfterInBackground.Start());
            return andAfterInBackground;
        }

        IAwaitRuntime AndAfterInMain(WaitAction action) {
            var andAfterInMain = new MainAwaitRuntime(action, false);
            AndAfterFinishDo(() => andAfterInMain.Start());
            return andAfterInMain;
        }

        static IAwaitRuntime WaitForSeconds(WaitAction toExecute, float seconds, bool autoStart = true) {
            return new WaitForSecondsAwaitRuntime(toExecute, seconds, autoStart);
        }

        static IAwaitRuntime WaitUntil(WaitUntilAwaitRuntime.WaitCondition after, WaitAction execute, bool autoStart = true) {
            return new WaitUntilAwaitRuntime(execute, after, autoStart);
        }

        static IAwaitRuntime EverySecondsDo(WaitAction execute,
            EverySecondDoAwaitRuntime.Seconds seconds,
            EverySecondDoAwaitRuntime.CancelCondition toCancel = null,
            bool autoStart = true) {
            return new EverySecondDoAwaitRuntime(execute, seconds, toCancel, autoStart);
        }

        static IAwaitRuntime MoveToBackground(WaitAction action, bool autoStart = true) {
            return new BackgroundAwaitRuntime(action, autoStart);
        }

        static IAwaitRuntime MoveToMain(WaitAction action, bool autoStart = true) {
            return new MainAwaitRuntime(action, autoStart);
        }

        static IAwaitRuntime WaitForEndOfFrame(WaitAction toExecute, bool autoStart = true) {
            return new WaitForEndOfFrameAwaitRuntime(toExecute, autoStart);
        }
    }
}