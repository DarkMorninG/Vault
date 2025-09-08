using System.Collections.Generic;
using System.Linq;
using UnityEngine.VFX;

namespace Vault {
    public static class VaultVisualEffect {
        public static bool IsPlaying(this VisualEffect me) {
            var list = new List<string>();
            me.GetSpawnSystemNames(list);
            return list.Select(me.GetSpawnSystemInfo).Any(state => state.playing);
        }
    }
}