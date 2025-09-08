using System.Collections.Generic;

namespace Vault {
    public static class VaultLong {
        public static Stack<long> Digits(this long value) {
            if (value == 0) return new Stack<long>();
            var numbers = Digits(value / 10);
            numbers.Push(value % 10);
            return numbers;
        }
    }
}