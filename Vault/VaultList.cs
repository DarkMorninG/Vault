using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vault.Dtos;
using Vault.Util;
using Random = System.Random;

namespace Vault {
    public static class VaultList {
        private static readonly Random rnd = new();


        public static void ReplaceOrAdd<T>(this List<T> list, T toReplace) {
            var findIndex = list.FindIndex(item => item.Equals(toReplace));
            if (findIndex == -1) {
                list.Add(toReplace);
                return;
            }

            list[findIndex] = toReplace;
        }

        public static bool IsEmpty<T>(this List<T> list) {
            if (list == null) return true;
            return !list.Any() || list.Count == 0 || list.Any(i => i == null);
        }

        public static bool IsGameobjectEmpty(this List<GameObject> list) {
            if (list.IsNullOrEmpty()) {
                return true;
            }

            var isEmpty = true;
            list.ForEach(o => {
                if (o != null) {
                    isEmpty = false;
                }
            });
            return isEmpty;
        }

        public static void AddRangeNoDuplicates<T>(this List<T> list, List<T> toBeAdded) {
            var copy = toBeAdded.Copy();
            copy.RemoveAll(list.Contains);
            list.AddRange(copy);
        }

        public static void AddNoDuplicate<T>(this List<T> list, T toBeAdded) {
            if (list.IsEmpty()) {
                list.Add(toBeAdded);
            }

            if (!list.Contains(toBeAdded)) {
                list.Add(toBeAdded);
            }
        }

        public static int CountNotNull<T>(this List<T> list) {
            var count = 0;
            if (list.IsEmpty()) return 0;

            list.ForEach(t => {
                if (!t.Equals(null)) count++;
            });
            return count;
        }

        public static List<T> GetNotNullItems<T>(this List<T> list) {
            var returnList = new List<T>();
            if (list.IsEmpty()) return null;
            list.ForEach(t => {
                if (!t.Equals(null)) returnList.Add(t);
            });
            return returnList;
        }

        public static List<T> RemoveEveryItemBefore<T>(this List<T> list, T target) {
            if (list.Count == 1) return default;
            var @new = list.Copy();
            var index = @new.FindIndex(x1 => x1.Equals(target));
            for (var i = 0; i <= index; i++) {
                @new.RemoveAt(0);
            }

            return @new;
        }

        public static void RemoveEveryItemAfter<T>(this List<T> list, T target) {
            if (list.Count == 1) return;
            var index = list.FindIndex(x => x.Equals(target));
            for (var i = index + 1; i <= list.Count; i++) {
                list.RemoveAt(index + 1);
            }
        }

        public static T GetRandomItem<T>(this List<T> list) {
            return list.IsEmpty() ? default : list[rnd.Next(0, list.Count)];
        }
        
        public static T GetRandomItem<T>(this List<T> list, Random rnd) {
            return list.IsEmpty() ? default : list[rnd.Next(0, list.Count)];
        }

        public static List<T> Copy<T>(this List<T> list) {
            var newList = new List<T>();
            newList.AddRange(list);
            return newList;
        }

        public static List<T> ReverseAndReturn<T>(this List<T> list) {
            var newList = list.Copy();
            newList.Reverse();
            return newList;
        }

        public static List<T> RemoveNullItems<T>(this List<T> list) {
            list.RemoveAll(g => g == null || g.Equals(null));
            return list;
        }

        public static T LastItem<T>(this List<T> list) {
            return list.IsEmpty() ? default : list[list.Count - 1];
        }

        public static T FirstItem<T>(this List<T> list) {
            return list.IsEmpty() ? default : list[0];
        }

        public static void RemoveLastItem<T>(this List<T> list) {
            if (list.IsEmpty()) return;
            list.RemoveAt(list.Count - 1);
        }

        public static void AddNotNull<T>(this List<T> list, T toBeAdded) {
            if (toBeAdded != null) {
                list.Add(toBeAdded);
            }
        }

        public static void AddRangeNotNull<T>(this List<T> list, IEnumerable<T> range) {
            var enumerable = range.ToList();
            if (enumerable.IsEmpty()) return;
            list.AddRange(enumerable.Where(r => !r.Equals(null)));
        }

        public static bool IsNullOrEmpty<T>(this List<T> list) {
            return list == null || list.IsEmpty();
        }

        public static IEnumerable<T> OrderByPositionInHierarchy<T>(this IEnumerable<T> enumerable) where T : Component {
            return enumerable.OrderBy(comp => new HierarchySortable(comp.gameObject));
        }

        public static Optional<T> FindOptional<T>(this List<T> source, Predicate<T> predicate) {
            var find = source.Find(predicate);
            return find == null ? Optional<T>.Empty() : Optional<T>.Of(find);
        }

        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize) {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static void Remove<T>(this List<T> source, List<T> toRemove) {
            foreach (var x1 in toRemove) {
                source.Remove(x1);
            }
        }

        public static ListDiffDto<T> Diff<T>(this List<T> source, List<T> toDiff) {
            var added = new List<T>();
            var removed = new List<T>();
            source.ForEach(obj => {
                if (!toDiff.Contains(obj)) {
                    added.Add(obj);
                }
            });
            toDiff.ForEach(obj => {
                if (!source.Contains(obj)) {
                    removed.Add(obj);
                }
            });
            return new ListDiffDto<T>(removed, added);
        }
    }
}