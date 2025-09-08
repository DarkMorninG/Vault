using System;

namespace Vault {
    public static class VaultEnum {
        public static string GetStringValue(this Enum value) {
            var stringValue = value.ToString();
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attrs = fieldInfo.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs.Length > 0) stringValue = attrs[0].Value;

            return stringValue;
        }
    }
}