namespace Vault {
    public static class VaultString {
        public static bool IsEmpty(this string me) {
            if (me == null) return true;
            if (me != "") return false;
            return me.Length == 0;
        }

        public static bool IsNotEmpty(this string me) {
            return !IsEmpty(me);
        }
        
        public static string ToLowerFirstChar(this string input)
        {
            if(string.IsNullOrEmpty(input))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}