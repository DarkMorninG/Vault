using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vault {
    public static class VaultGameObject {
        public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component {
            return go.AddComponent<T>().GetCopyOf(toAdd);
        }

        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject) {
            var transform = gameObject.transform;
            var foundChildren = new List<GameObject>();
            for (var i = 0; i < transform.childCount; i++) foundChildren.Add(transform.GetChild(i).gameObject);

            return foundChildren;
        }

        public static T GetComponentInChildrenForce<T>(this GameObject gameObject) where T : Component {
            var findAllChildrenRecursive = FindAllChildrenRecursive(gameObject);
            return findAllChildrenRecursive.Select(o => o.GetComponent<T>())
                .FirstOrDefault(component => component != null);
        }

        public static IEnumerable<GameObject> FindAllChildrenRecursive(this GameObject root) {
            var result = new List<GameObject>();
            if (root.transform.childCount <= 0) return result;
            foreach (Transform variable in root.transform) {
                Searcher(result, variable.gameObject);
            }

            return result;
        }

        private static void Searcher(ICollection<GameObject> list, GameObject root) {
            list.Add(root);
            if (root.transform.childCount <= 0) return;
            foreach (Transform variable in root.transform) {
                Searcher(list, variable.gameObject);
            }
        }

        public static void AddChild(this GameObject gameObject, GameObject child) {
            if (child == null) return;
            child.transform.parent = gameObject.transform;
        }

        public static void CopyComponents(this GameObject gameObject, GameObject from) {
            foreach (var component in from.GetComponents(typeof(Component)))
                if (component.GetType() != typeof(Transform))
                    gameObject.AddComponent(component);
        }

        public static T GetInterfaceComponent<T>(this GameObject gameObject) where T : class {
            return gameObject.GetComponent(typeof(T)) as T;
        }

        public static List<T> FindObjectsOfInterface<T>(this GameObject gameObject) where T : class {
            var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            var list = new List<T>();
            foreach (var behaviour in monoBehaviours)
                if (behaviour.GetComponent(typeof(T)) is T component)
                    list.Add(component);

            return list;
        }

        public static IEnumerable<T> FindGameobjectsWithComponent<T>(this GameObject me) where T : Component {
            var foundComponent = (from gameObject in me.Children()
                where gameObject.GetComponent<T>()
                select gameObject.GetComponent<T>()).ToList();
            return foundComponent;
        }

        public static T FindFirstGameobjectWithComponent<T>(this GameObject me) where T : Component {
            foreach (var gameObject in me.Children())
                if (gameObject.GetComponent<T>())
                    return gameObject.GetComponent<T>();

            return default;
        }

        public static IEnumerable<GameObject> Children(this GameObject me) {
            var children = new List<GameObject>();
            for (var i = 0; i < me.transform.childCount; i++) children.Add(me.transform.GetChild(i).gameObject);

            return children;
        }


//    public static List<GameObject> FindGameObjectsWithTagInScene(this GameObject me, string tag) {
//        var objects = new List<GameObject>();
//        foreach (var rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects()) {
//            if (rootGameObject.tag.Equals(tag)) {
//                objects.Add(rootGameObject);
//            } else {
//                if (rootGameObject.transform.childCount > 0) {
//                    
//                }
//            }
//        }
//
//        return objects;
//    }
    }
}