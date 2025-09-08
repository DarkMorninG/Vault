using System.Collections.Generic;
using UnityEngine;

namespace Vault.Debug {
    public class DebugDrawVO : ScriptableObject {
        public List<Vector3> Points;
        public Color Color;
        public bool AutoDelete;
    }
}