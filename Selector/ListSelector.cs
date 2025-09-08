using System.Collections.Generic;

namespace Vault.Selector {
    public class ListSelector<T> {
        private readonly List<T> _currentList;

        public delegate void ChangeSelection(T newSelected, T oldSelected);

        public event ChangeSelection OnSelectionChange;

        private bool _enableRoundRobin = true;

        public bool EnableRoundRobin {
            get => _enableRoundRobin;
            set => _enableRoundRobin = value;
        }

        public ListSelector(List<T> currentList, T firstItem) {
            _currentList = currentList;
            currentList.FindOptional(obj => obj.Equals(firstItem))
                .IfPresent(obj => Selected = obj);
        }

        public ListSelector(List<T> currentList) {
            _currentList = currentList;
            if (!_currentList.IsNullOrEmpty()) {
                Selected = currentList[0];
            }
        }

        public T Selected { get; private set; }

        public List<T> CurrentList => _currentList;

        public void Select(T toSelect) {
            var oldSelected = Selected;
            Selected = toSelect;
            OnSelectionChange?.Invoke(Selected, oldSelected);
        }


        public T Increase() {
            if (_currentList.IsEmpty()) return default;
            var currentIndex = _currentList.IndexOf(Selected);
            T currentSelected;
            if (currentIndex + 1 > _currentList.Count - 1)
                currentSelected = _enableRoundRobin ? _currentList[0] : default;
            else
                currentSelected = _currentList[currentIndex + 1];
            Select(currentSelected);
            return Selected;
        }

        public T Decrease() {
            if (_currentList.IsEmpty()) return default;
            var currentIndex = _currentList.IndexOf(Selected);
            T currentSelected;
            if (currentIndex > 0)
                currentSelected = _currentList[currentIndex - 1];
            else
                currentSelected = _enableRoundRobin ? _currentList[^1] : default;
            Select(currentSelected);
            return Selected;
        }
    }
}