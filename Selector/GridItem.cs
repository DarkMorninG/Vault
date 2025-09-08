namespace Vault.Selector {
    public class GridItem<T> {
        private int row;
        private int column;
        private T item;
        
        public GridItem(int row, int column, T item) {
            this.row = row;
            this.column = column;
            this.item = item;
        }
        
        public int Row => row;
        
        public int Column => column;
        
        public T Item => item;
    }
}