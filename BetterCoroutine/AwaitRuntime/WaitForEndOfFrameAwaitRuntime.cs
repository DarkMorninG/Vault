using System;
using UnityEngine;

namespace BetterCoroutine.AwaitRuntime {
    public class WaitForEndOfFrameAwaitRuntime : IAwaitRuntime {
        private readonly IAwaitRuntime.WaitAction action;
        private bool isRunning;
        private bool isFinished;
        private IAwaitRuntime.WaitAction afterFinished;

        public bool Running => isRunning;
        public bool IsFinished => isFinished;

        public WaitForEndOfFrameAwaitRuntime(IAwaitRuntime.WaitAction action, bool autoStart = true) {
            this.action = action;
            if (autoStart) Start();
        }

        public async void Start() {
            try {
                isRunning = true;
                await Awaitable.EndOfFrameAsync();
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
            throw new NotImplementedException();
        }

        public void AndAfterFinishDo(IAwaitRuntime.WaitAction afterFinished) {
            this.afterFinished = afterFinished;
        }
    }
}