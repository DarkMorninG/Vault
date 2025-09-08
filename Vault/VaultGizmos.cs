using UnityEngine;

namespace Vault {
    public static class VaultGizmos {
        public static void DrawWireDisk(Vector3 position, float radius, Color color) {
            var oldColor = Gizmos.color;
            Gizmos.color = color;
            var oldMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 0f, 1));
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;
        }

        public static void DrawArrow(Vector3 pos,
            Vector3 direction,
            Color? color = null,
            float arrowHeadLength = 0.25f,
            float arrowHeadAngle = 20.0f) {
            Gizmos.color = color ?? Color.white;

            //arrow shaft
            Gizmos.DrawRay(pos, direction);
 
            if (direction != Vector3.zero)
            {
                //arrow head
                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
                Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
                Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            }
        }
    }
}