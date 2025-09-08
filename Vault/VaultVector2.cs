using System;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Vault {
    public static class VaultVector2 {
        public static Vector3 ConvertTo3DSpace(this Vector2 me) {
            return me;
        }

        public static Vector3 ConvertTo3DSpaceWithZ(this Vector2 me) {
            return new Vector3(me.x, 0f, me.y);
        }

        public static Vector2 Minimize(this Vector2 me) {
            var minimizedVector = new Vector2();
            if (me.x > 1) minimizedVector.x = 1;

            if (me.x < 1) minimizedVector.x = -1;

            if (me.y > 1) minimizedVector.y = 1;

            if (me.y < -1) minimizedVector.y = -1;

            return minimizedVector;
        }

        public static bool TargetReached(this Vector2 me, Vector2 target, float maxTollerance) {
            var meX = Math.Abs(me.x);
            var meY = Math.Abs(me.y);
            var taX = Math.Abs(target.x);
            var taY = Math.Abs(target.y);

            var allMatched = FloatComparer.AreEqual(taX, meX, maxTollerance) &&
                             FloatComparer.AreEqual(taY, meY, maxTollerance);
            return allMatched;
        }

        public static Vector2 RotateBy(this Vector2 v, float a, bool bUseRadians = false) {
            if (!bUseRadians) a *= Mathf.Deg2Rad;
            var ca = Math.Cos(a);
            var sa = Math.Sin(a);
            var rx = v.x * ca - v.y * sa;

            return new Vector2((float)rx, (float)(v.x * sa + v.y * ca));
        }
        
    }
}