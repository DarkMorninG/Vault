using System;
using UnityEngine;

namespace BetterCoroutine.AwaitRuntime {
    public class BackgroundAwaitRuntime : IAwaitRuntime {
        private readonly IAwaitRuntime.WaitAction action;
        private bool isRunning;
        private bool isFinished;
        private IAwaitRuntime.WaitAction afterFinished;

        public bool Running => isRunning;
        public bool IsFinished => isFinished;

        public BackgroundAwaitRuntime(IAwaitRuntime.WaitAction action, bool autoStart = true) {
            this.action = action;
            if (autoStart) Start();
        }

        public async void Start() {
            try {
                isRunning = true;
                await Awaitable.BackgroundThreadAsync();
                action?.Invoke();
                isRunning = false;
                afterFinished?.Invoke();
            }
            catch (Exception e) {
                isRunning = false;
                Debug.LogException(e);
            }
        }

        public void Stop() {
            throw new NotImplementedException();
        }

        public void AndAfterFinishDo(IAwaitRuntime.WaitAction afterFinished) {
            this.afterFinished = afterFinished;
        }
    }
}