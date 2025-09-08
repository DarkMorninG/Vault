using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vault {
    public static class VaultVector3 {
        public static Vector2 ConvertTo2D(this Vector3 me) {
            return new Vector2(me.x, me.y);
        }

        public static Vector3 Invert(this Vector3 me) {
            return me * -1f;
        }

        public static bool HasNegativeValue(this Vector3 me) {
            return (me.z < 0) | (me.y < 0) | (me.x < 0);
        }

        public static Vector3 Abs(this Vector3 me) {
            return new Vector3(Math.Abs(me.x), Math.Abs(me.y), Math.Abs(me.z));
        }

        public static Vector3 CreateNew(this Vector3 me) {
            return new Vector3(me.x, me.y, me.z);
        }
        
        
        public static Vector3 JustBeforeTarget(this Vector3 me, Vector3 to) {
            return Vector3.Lerp(me, to, .85f);
        }
        
        public static List<Vector3> CirclePositionAround(this Vector3 me, int howMany) {
            var circlePositions = new List<Vector3>();
            var radius = howMany;
            for (var i = 0; i < howMany; i++) {
                var angle = i * Mathf.PI * 2f / radius;
                var newPos = me +
                             (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
                circlePositions.Add(newPos);
            }

            return circlePositions;
        }


        public static bool TargetReached(this Vector3 me, Vector3 target, float maxTolerance) {
            var distance = Vector3.Distance(me, target);
            return distance <= maxTolerance;
        }
        
        public static bool TargetReachedWithoutHeight(this Vector3 me, Vector3 target, float maxTolerance) {
            var meCopy = me;
            var targetCopy = target;
            meCopy.y = 0;
            targetCopy.y = 0;
            var distance = Vector3.Distance(meCopy, targetCopy);
            return distance <= maxTolerance;
        }
    }
}