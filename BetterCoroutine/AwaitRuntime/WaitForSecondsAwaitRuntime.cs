using System;
using System.Threading;
using UnityEngine;

namespace BetterCoroutine.AwaitRuntime {
    public class WaitForSecondsAwaitRuntime : IAwaitRuntime {
        private readonly IAwaitRuntime.WaitAction action;
        private readonly float seconds;
        private bool isRunning;
        private bool isFinished;
        private IAwaitRuntime.WaitAction afterFinished;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;

        public bool Running => isRunning;
        public bool IsFinished => isFinished;


        public WaitForSecondsAwaitRuntime(IAwaitRuntime.WaitAction action, float seconds, bool autoStart = true) {
            this.action = action;
            this.seconds = seconds;
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            if (autoStart) Start();
        }

        public async void Start() {
            try {
                isRunning = true;
                await Awaitable.WaitForSecondsAsync(seconds, cancellationToken);
                action?.Invoke();
                isRunning = false;
                afterFinished?.Invoke();
                isFinished = true;
            }
            catch (Exception e) {
                isRunning = false;
                Debug.LogException(e);
            }
        }

        public void Stop() {
            cancellationTokenSource.Cancel();
            isFinished = false;
        }

        public void AndAfterFinishDo(IAwaitRuntime.WaitAction afterFinished) {
            this.afterFinished = afterFinished;
        }
    }
}