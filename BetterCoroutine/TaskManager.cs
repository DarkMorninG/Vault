/// Simple, really.  There is no need to initialize or even refer to TaskManager.
/// When the first Task is created in an application, a "TaskManager" GameObject
/// will automatically be added to the scene root with the TaskManager component
/// attached.  This component will be responsible for dispatching all coroutines
/// behind the scenes.
///
/// Task also provides an event that is triggered when the coroutine exits.

using System.Collections;
using UnityEngine;

namespace Vault.BetterCoroutine {
    internal class TaskManager : MonoBehaviour {
        private static TaskManager _singleton;

        public static TaskState CreateTask(IEnumerator coroutine) {
            if (_singleton != null) return new TaskState(coroutine);
            var go = new GameObject("TaskManager");
            _singleton = go.AddComponent<TaskManager>();

            return new TaskState(coroutine);
        }

        public class TaskState {
            public delegate void FinishedHandler(bool manual);

            private readonly IEnumerator coroutine;

            private bool _stopped;

            public TaskState(IEnumerator c) {
                coroutine = c;
            }

            public bool Running { get; private set; }

            public bool Paused { get; private set; }

            public bool Stopped => _stopped;

            public event FinishedHandler OnFinished;

            public void Pause() {
                Paused = true;
            }

            public void Unpause() {
                Paused = false;
            }

            public void Start() {
                Running = true;
                _singleton.StartCoroutine(CallWrapper());
            }

            public void Stop() {
                _stopped = true;
                Running = false;
                OnFinished?.Invoke(_stopped);
            }

            private IEnumerator CallWrapper() {
                yield return null;
                var enumerator = coroutine;
                while (Running)
                    if (Paused) {
                        yield return null;
                    } else {
                        if (enumerator != null && enumerator.MoveNext())
                            yield return enumerator.Current;
                        else
                            Running = false;
                    }

                if (!_stopped) {
                    OnFinished?.Invoke(_stopped);
                }
            }
        }
    }
}