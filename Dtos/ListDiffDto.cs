using System.Collections.Generic;

namespace Vault.Dtos {
    public class ListDiffDto<T> {
        private List<T> missing;
        private List<T> added;

        public List<T> Missing => missing;

        public List<T> Added => added;

        public ListDiffDto(List<T> missing, List<T> added) {
            this.missing = missing;
            this.added = added;
        }
        
        
    }
}