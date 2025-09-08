using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Vault {
    public static class VaultTransform {
        public static void ChangePositionX(this Transform me, float x) {
            var position = me.position;
            position = new Vector3(x, position.y, position.z);
            me.position = position;
        }
        
        public static void ChangePositionY(this Transform me, float y) {
            var position = me.position;
            position = new Vector3(position.x, y, position.z);
            me.position = position;
        }
        
        public static void ChangePositionZ(this Transform me, float z) {
            var position = me.position;
            position = new Vector3(position.x, position.y, z);
            me.position = position;
        }
        
        public static void ChangePositionLocalX(this Transform me, float x) {
            var position = me.localPosition;
            position = new Vector3(x, position.y, position.z);
            me.localPosition = position;
        }
        
        public static void ChangePositionLocalY(this Transform me, float y) {
            var position = me.localPosition;
            position = new Vector3(position.x, y, position.z);
            me.localPosition = position;
        }
        
        public static void ChangePositionLocalZ(this Transform me, float z) {
            var position = me.localPosition;
            position = new Vector3(position.x, position.y, z);
            me.localPosition = position;
        }
        
        public static void DeleteAllChildren(this Transform me) {
            //TODO work around since its looks like deleting is async
            while (me.childCount > 0)
                foreach (Transform o in me)
                    Object.DestroyImmediate(o.gameObject);
        }
        
        public static void RotateTorwads2D(this Transform me, Vector3 target, float turnSpeed) {
            var direction = target - me.position;
            var rotation = Quaternion.LookRotation(Vector3.forward, direction);
            me.rotation = Quaternion.Lerp(me.rotation, rotation, Time.deltaTime * turnSpeed);
        }
        
        public static void RotateTorwads2D(this LocalTransform me, float3 target, float turnSpeed, float deltaTime) {
            var direction = target - me.Position;
            var rotation = Quaternion.LookRotation(Vector3.forward, direction);
            me.Rotation = Quaternion.Lerp(me.Rotation, rotation, deltaTime * turnSpeed);
        }
        
        public static void RotateTorwads2D(this Transform me, Vector3 target) {
            var direction = target - me.position;
            me.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }   
        
        public static void RotateTorwads2D(this LocalTransform me, float3 target) {
            var direction = target - me.Position;
            me.Rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }        
        
        public static Vector3 backward(this Transform me) {
            return me.forward * -1;
        }
    }
}