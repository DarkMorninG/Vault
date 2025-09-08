using System;

namespace Vault {
    public static class VaultRandom {
        public static float NextFloat(this Random random, float min, float max) {
            return (float)random.NextDouble() * (max - min) + min;
        }
    }
}