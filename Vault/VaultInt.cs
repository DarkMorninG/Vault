using System.Collections.Generic;

namespace Vault {
    public static class VaultInt {
        public static Stack<int> Digits(this int value) {
            if (value == 0) return new Stack<int>();
            var numbers = Digits(value / 10);
            numbers.Push(value % 10);
            return numbers;
        }
    }
}