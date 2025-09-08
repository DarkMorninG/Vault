namespace Vault.Selector {
    public interface ISelector<T> {
        public delegate void SelectionChange(T nextSelect, T oldSelected);

        public event SelectionChange OnSelectionChange;

        T CurrentSelected { get; }
    }
}