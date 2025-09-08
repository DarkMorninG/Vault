using Unity.Mathematics;
using UnityEngine;

namespace Vault {
    public static class VaultFloat3 {
        public static Vector3 toVector3(this float3 me) {
            return new Vector3(me.x, me.y, me.z);
        }
    }
}