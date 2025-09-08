using System.Collections.Generic;

namespace Vault {
    public static class Lists {
        public static List<T> Of<T>(params T[] toadd) {
            var list = new List<T>();
            list.AddRange(toadd);
            return list;
        }
        public static List<T> Empty<T>() {
            return new List<T>();
        }

    }
}