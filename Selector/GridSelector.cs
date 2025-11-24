using System.Collections.Generic;
using System.Linq;

namespace Vault.Selector {
    public class GridSelector<T> : ISelector<T> {
        private List<GridItem<T>> gridItems;

        private int currentRow;
        private int currentColumn;
        private int rowLength;
        private int columnLength;

        private T currentSelect;

        private bool roundRobin;


        public event ISelector<T>.SelectionChange OnSelectionChange;
        public T CurrentSelected => currentSelect;

        public List<T> GridItems => gridItems.Select(item => item.Item).ToList();


        public bool RoundRobin {
            get => roundRobin;
            set => roundRobin = value;
        }

        public GridSelector(List<GridItem<T>> gridItems) {
            this.gridItems = gridItems;
            rowLength = gridItems.Max(item => item.Row);
            columnLength = gridItems.Max(item => item.Column);
            currentSelect = gridItems[0].Item;
        }

        public GridSelector(List<GridItem<T>> gridItems, T start) {
            currentSelect = start;
            this.gridItems = gridItems;
            rowLength = gridItems.Max(item => item.Row);
            columnLength = gridItems.Max(item => item.Column);
            gridItems.FindOptional(item => item.Item.Equals(start))
                .IfPresent(gridItem => {
                    currentRow = gridItem.Row;
                    currentColumn = gridItem.Column;
                });
        }


        public T Right() {
            if (currentRow == rowLength && roundRobin) {
                var maxColumnInRow = gridItems.Where(item => item.Row == currentRow).Max(item => item.Column);
                if (currentColumn == maxColumnInRow) {
                    currentColumn = 0;
                    currentRow = 0;
                } else {
                    currentColumn += 1;
                }
            } else {
                if (currentColumn + 1 > columnLength) {
                    if (roundRobin) {
                        currentColumn = 0;
                        currentRow += 1;
                    }
                } else {
                    currentColumn++;
                }
            }


            UpdateSelection(currentColumn, currentRow);
            return currentSelect;
        }

        public T Left() {
            if (currentRow == 0 && currentColumn == 0 && roundRobin) {
                var maxColumnInRow = gridItems.Where(item => item.Row == rowLength).Max(item => item.Column);
                currentColumn = maxColumnInRow;
                currentRow = rowLength;
            } else {
                if (currentColumn - 1 < 0) {
                    if (roundRobin) {
                        currentRow -= 1;
                        currentColumn = columnLength;
                    }
                } else {
                    currentColumn--;
                }
            }

            UpdateSelection(currentColumn, currentRow);
            return currentSelect;
        }

        public T Down() {
            if (currentRow + 1 > rowLength) {
                if (roundRobin) {
                    currentRow = 0;
                }
            } else {
                currentRow++;
            }

            if (!gridItems.Any(item => item.Row == currentRow && item.Column == currentColumn) && roundRobin) {
                currentColumn = gridItems.Where(item => item.Row == currentRow).Max(item => item.Column);
            }

            UpdateSelection(currentColumn, currentRow);
            return currentSelect;
        }

        public T Up() {
            if (currentRow - 1 < 0) {
                if (roundRobin) {
                    currentRow = rowLength;
                    if (!gridItems.Any(item => item.Row == currentRow && item.Column == currentColumn)) {
                        currentColumn = 0;
                    }
                }
            } else {
                currentRow--;
            }

            if (!gridItems.Any(item => item.Row == currentRow && item.Column == currentColumn) && roundRobin) {
                currentColumn = gridItems.Where(item => item.Row == currentRow).Max(item => item.Column);
            }

            UpdateSelection(currentColumn, currentRow);
            return currentSelect;
        }

        private void UpdateSelection(int column, int row) {
            var newSelected = gridItems.Find(item => item.Column == column && item.Row == row).Item;
            OnSelectionChange?.Invoke(newSelected, currentSelect);
            currentSelect = newSelected;
        }
    }
}