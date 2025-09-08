namespace Vault.BetterCoroutine {
    public static class RuntimeExtend {
        /// <summary>
        /// null safe Check on Running boolean
        /// </summary>
        public static bool IsRunning(this IAsyncRuntime me) {
            return me is { Running: true };
        }

        public static bool IsNotRunning(this IAsyncRuntime me) {
            return !(me.IsRunning());
        }
    }
}