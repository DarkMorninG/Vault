using System.Collections.Generic;
using UnityEngine;
using Vault.Debug;

namespace Vault {
    public class Draw : MonoBehaviour {
        public List<DebugDrawVO> Vos = new List<DebugDrawVO>();
        private List<DebugDrawVO> _removing = new List<DebugDrawVO>();


        public void Line(Color color, params Vector3[] point) {
            var vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            var p = new List<Vector3>();
            int i = 0, l = point.Length;
            for (; i < l; ++i) {
                p.Add(point[i]);
            }

            vo.Points = p;
            vo.Color = color;
            vo.AutoDelete = true;
            Vos.Add(vo);
        }

        public void Line(Color color, List<Vector3> points, bool autoDelete = true) {
            var vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            vo.Points = points;
            vo.Color = color;
            vo.AutoDelete = autoDelete;
            Vos.Add(vo);
        }

        public void MarkPoint(Color color, Vector3 pos, bool autoDelete = true) {
            Line(color, Lists.Of(pos, Vector3.up + pos), autoDelete);
        }

        public void AABB(Color color, GameObject gameObject, bool autoDelete = true) {
            if (gameObject.transform.GetComponent<Renderer>() == null) {
                return;
            }

            var vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            var min = gameObject.transform.GetComponent<Renderer>().bounds.min;
            var max = gameObject.transform.GetComponent<Renderer>().bounds.max;

            var p = new List<Vector3>();
            p.Add(new Vector3(min.x, min.y, min.z));
            p.Add(new Vector3(min.x, min.y, max.z));

            p.Add(new Vector3(min.x, min.y, max.z));
            p.Add(new Vector3(max.x, min.y, max.z));

            p.Add(new Vector3(max.x, min.y, max.z));
            p.Add(new Vector3(max.x, min.y, min.z));

            p.Add(new Vector3(max.x, min.y, min.z));
            p.Add(new Vector3(min.x, min.y, min.z));

            p.Add(new Vector3(min.x, max.y, min.z));
            p.Add(new Vector3(min.x, max.y, max.z));

            p.Add(new Vector3(min.x, max.y, max.z));
            p.Add(new Vector3(max.x, max.y, max.z));

            p.Add(new Vector3(max.x, max.y, max.z));
            p.Add(new Vector3(max.x, max.y, min.z));

            p.Add(new Vector3(max.x, max.y, min.z));
            p.Add(new Vector3(min.x, max.y, min.z));

            p.Add(new Vector3(max.x, max.y, min.z));
            p.Add(new Vector3(max.x, min.y, min.z));

            p.Add(new Vector3(max.x, min.y, max.z));
            p.Add(new Vector3(max.x, max.y, max.z));

            p.Add(new Vector3(min.x, max.y, max.z));
            p.Add(new Vector3(min.x, min.y, max.z));

            vo.Points = p;
            vo.Color = color;
            vo.AutoDelete = autoDelete;
            Vos.Add(vo);
        }

        public void Circle(Color color, Vector3 point, float radius, Quaternion rotation, bool autoDelete = true) {
            Circle(color, point, radius, rotation.eulerAngles, autoDelete);
        }

        public void Circle(Color color, Vector3 point, float radius, Vector3 rotation, bool autoDelete = true) {
            int segments = (int)(radius * 50);
            float angle = (360f) / (float)segments;
            float a = angle;
            List<Vector3> points = new List<Vector3>();

            int i = 0, l = segments + 1;

            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);

            for (; i < l; ++i) {
                Vector3 pos = Vector3.zero;
                float ca = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float cb = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                pos = new Vector3(ca, cb, 0);
                pos = m.MultiplyPoint3x4(pos);
                pos.x += point.x;
                pos.y += point.y;
                pos.z += point.z;

                points.Add(pos);

                angle += a;
            }

            DebugDrawVO vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            vo.Points = points;
            vo.Color = color;
            vo.AutoDelete = autoDelete;
            Vos.Add(vo);
        }

        public void Sphere(Color color, Vector3 point, float radius, bool autoDelete = true) {
            int segments = (int)(radius * 50);
            float angle = (360f) / (float)segments;
            float a = angle;
            List<Vector3> pointsa = new List<Vector3>();
            List<Vector3> pointsb = new List<Vector3>();
            List<Vector3> pointsc = new List<Vector3>();

            int i = 0, l = (segments * 3) + 3;
            for (; i < l; ++i) {
                Vector3 pos = Vector3.zero;

                float ca = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
                float cb = radius * Mathf.Cos(angle * Mathf.Deg2Rad);

                if (i <= segments) {
                    pos = new Vector3(point.x + ca, point.y + cb, point.z);
                    pointsa.Add(pos);
                } else if (i <= (segments * 2) + 1) {
                    pos = new Vector3(point.x + ca, point.y, point.z + cb);
                    pointsb.Add(pos);
                } else {
                    pos = new Vector3(point.x, point.y + ca, point.z + cb);
                    pointsc.Add(pos);
                }


                angle += a;
            }

            DebugDrawVO vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            vo.Points = pointsa;
            vo.Color = color;
            vo.AutoDelete = autoDelete;
            Vos.Add(vo);

            vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            vo.Points = pointsb;
            vo.Color = color;
            vo.AutoDelete = autoDelete;
            Vos.Add(vo);

            vo = ScriptableObject.CreateInstance<DebugDrawVO>();
            vo.Points = pointsc;
            vo.Color = color;
            vo.AutoDelete = autoDelete;
            Vos.Add(vo);
        }

        private void RemoveOldData(List<DebugDrawVO> vosToRemove) {
            int i = 0, l = vosToRemove.Count;
            for (; i < l; ++i) {
                int index = Vos.IndexOf(vosToRemove[i]);
                Destroy(Vos[index]);
                Vos.RemoveAt(index);
            }
        }

        //Do the actual debug stuff in gizmos
        void OnDrawGizmos() {
            if (!UnityEngine.Debug.isDebugBuild) {
                return;
            }

            _removing = new List<DebugDrawVO>();

            int i = 0, l = Vos.Count;
            for (; i < l; ++i) {
                DebugDrawVO vo = Vos[i];
                if (vo.Points != null) {
                    Gizmos.color = vo.Color;
                    int j = 1, k = vo.Points.Count;
                    for (; j < k; ++j) {
                        Gizmos.DrawLine(vo.Points[j - 1], vo.Points[j]);
                    }
                }

                if (vo.AutoDelete) {
                    _removing.Add(vo);
                }
            }
        }

        //need to remove the data outside of OnDrawGizmos so they draw in the game view
        void LateUpdate() {
            RemoveOldData(_removing);
        }
    }
}