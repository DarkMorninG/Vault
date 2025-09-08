using System.Collections.Generic;
using System.Linq;

namespace Vault {
    public static class VaultSet {
        public static bool IsEmpty<T>(this ISet<T> set) {
            if (set == null) return true;
            return !set.Any() || set.Count == 0 || set.Any(i => i == null);
        }

        public static void RemoveLastItem<T>(this ISet<T> set) {
            if (set.IsEmpty()) return;
            set.Remove(set.Last());
        }

        public static HashSet<T> Copy<T>(this HashSet<T> set) {
            var newSet = new HashSet<T>();
            foreach (var x1 in set) {
                newSet.Add(x1);
            }
            return newSet;
        }
    }
}