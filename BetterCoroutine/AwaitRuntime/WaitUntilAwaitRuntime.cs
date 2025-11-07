using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BetterCoroutine.AwaitRuntime {
    public class WaitUntilAwaitRuntime : IAwaitRuntime {
        public delegate bool WaitCondition();
        private readonly IAwaitRuntime.WaitAction toExecute;
        private readonly WaitCondition toWaitFor;
        private bool isRunning;
        private bool isFinished;
        private IAwaitRuntime.WaitAction afterFinished;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly CancellationToken cancellationToken;

        public bool Running => isRunning;
        public bool IsFinished => isFinished;

        public WaitUntilAwaitRuntime(IAwaitRuntime.WaitAction toExecute, WaitCondition toWaitFor, bool autoStart = true) {
            this.toExecute = toExecute;
            this.toWaitFor = toWaitFor;
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            if (autoStart) Start();
        }

        public async void Start() {
            try {
                isRunning = true;
                await UniTask.WaitUntil(() => toWaitFor.Invoke(), PlayerLoopTiming.Update, cancellationToken);
                toExecute.Invoke();
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
            isRunning = false;
        }

        public void AndAfterFinishDo(IAwaitRuntime.WaitAction afterFinished) {
            this.afterFinished = afterFinished;
        }
    }
}