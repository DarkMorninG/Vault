using System;
using System.Collections.Generic;
using System.Linq;

namespace Vault {
    public static class VaultEnumerable {
        public static IEnumerable<T> Peek<T>(this IEnumerable<T> source, Action<T> action) {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            return Iterator();

            IEnumerable<T> Iterator() // C# 7 Local Function
            {
                foreach (var item in source) {
                    action(item);
                    yield return item;
                }
            }
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> ie, Action<T, int> action) {
            var i = 0;
            foreach (var e in ie) {
                action(e, i++);
            }
        }
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action) {
            foreach (var e in ie) {
                action(e);
            }
        }


        public static Optional<T> LastOptional<T>(this IEnumerable<T> source) {
            return source.IsNullOrEmpty() ? Optional<T>.Empty() : Optional<T>.Of(source.Last());
        }

        public static Optional<T> FirstOptional<T>(this IEnumerable<T> source) {
            return source.IsNullOrEmpty() ? Optional<T>.Empty() : Optional<T>.Of(source.First());
        }
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source) {
            return source == null || !source.Any();
        }
    }
}