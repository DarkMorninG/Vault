using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vault.Util {
    public struct HierarchySortable : IComparable<HierarchySortable> {
        private List<int> _parentSiblingIndexes;

        public HierarchySortable(GameObject gameObject) : this() {
            _parentSiblingIndexes = new List<int>();

            if (gameObject.transform.parent == null) {
                _parentSiblingIndexes.Add(gameObject.transform.GetSiblingIndex());
            } else {
                Transform parent = gameObject.transform;

                while (parent) {
                    _parentSiblingIndexes.Add(parent.GetSiblingIndex());

                    parent = parent.parent;
                }
            }

            _parentSiblingIndexes.Reverse();
        }

        public int CompareTo(HierarchySortable other) {
            int aCount = this._parentSiblingIndexes.Count;
            int bCount = other._parentSiblingIndexes.Count;

            for (int i = 0; i < Math.Max(bCount, aCount); i++) {
                bool aMaxed = i >= aCount;
                bool bMaxed = i >= bCount;

                if (aMaxed && !bMaxed)
                    return -1;
                else if (!aMaxed && bMaxed)
                    return 1;

                int a = this._parentSiblingIndexes[i];
                int b = other._parentSiblingIndexes[i];

                if (a > b)
                    return 1;
                else if (a < b)
                    return -1;
            }

            return 0;
        }
    }
}