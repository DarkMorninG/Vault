using UnityEngine;

namespace Vault.Debug {
    public class VaultDebug {
        private static GameObject _instance;

        private static Draw _draw;

        public static Draw Draw {
            get {
                CreateInstance();
                return _draw;
            }
        }

        private static void CreateInstance() {
            if (_instance != null) return;
            var instance = new GameObject {
                name = "Debug Object"
            };
            _instance = instance;
            _draw = instance.AddComponent<Draw>();
        }
    }
}