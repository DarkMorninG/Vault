using System;
using System.Threading;
using UnityEngine;

namespace BetterCoroutine.AwaitRuntime {
    public class EverySecondDoAwaitRuntime : IAwaitRuntime {
        public delegate float Seconds();
        public delegate bool CancelCondition();
        private readonly IAwaitRuntime.WaitAction action;
        private readonly Seconds secondTimer;
        private readonly CancelCondition toCancel;
        private bool isRunning;
        private bool isFinished;
        private IAwaitRuntime.WaitAction afterFinished;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;

        private bool cancel;
        public bool Running => isRunning;
        public bool IsFinished => isFinished;

        public EverySecondDoAwaitRuntime(IAwaitRuntime.WaitAction action, Seconds secondTimer, CancelCondition toCancel = null, bool autoStart = true) {
            this.action = action;
            this.secondTimer = secondTimer;
            this.toCancel = toCancel;
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            if (autoStart) Start();
        }
        

        public async void Start() {
            try {
                isRunning = true;
                while (!cancel) {
                    await Awaitable.WaitForSecondsAsync(secondTimer.Invoke(), cancellationToken);
                    action?.Invoke();
                    cancel = toCancel?.Invoke() ?? false;
                }

                afterFinished?.Invoke();
                isRunning = false;
                isFinished = true;
            }
            catch (Exception e) {
                isRunning = false;
                if (e is OperationCanceledException) {
                    Debug.LogWarning($"WaitForSecondsAwaitRuntime was stopped from: {e.StackTrace}");
                } else {
                    Debug.LogException(e);
                }
            }
        }

        public void Stop() {
            cancel = true;
            cancellationTokenSource.Cancel();
            isFinished = false;
        }

        public void AndAfterFinishDo(IAwaitRuntime.WaitAction afterFinished) {
            this.afterFinished = afterFinished;
        }
    }
}