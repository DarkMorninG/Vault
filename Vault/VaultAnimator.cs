using UnityEngine;

namespace Vault {
    public static class VaultAnimator {
        public static bool IsPlaying(this Animator me, string stateName, int layer = 0) {
            return IsPlaying(me) && me.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        public static bool IsPlaying(this Animator me) {
            return me.GetCurrentAnimatorStateInfo(0).length >
                   me.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }
}