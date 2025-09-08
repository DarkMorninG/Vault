using System;
using System.Linq;
using System.Reflection;

namespace Vault {
    [Serializable]
    public abstract class BetterEnum {
        public abstract string Name { get; }
        
    }

    public static class BetterEnumExtension {
        public static BetterEnum[] Values(this BetterEnum me) {
            var type = me.GetType();
            if (!type.IsSubclassOf(typeof(BetterEnum))) {
                return null;
            }

            var betterEnums = type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == type)
                .Select(info => info.GetValue(type) as BetterEnum)
                .ToArray();
            return betterEnums;
        }
    }
}