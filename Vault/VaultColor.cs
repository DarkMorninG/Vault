using UnityEngine;

namespace Vault {
    public static class VaultColor {
        public static Color ChangeOpacity(this Color me, float opacity) {
            me.a = opacity;
            return me;
        }
    }
}