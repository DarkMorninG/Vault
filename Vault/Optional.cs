using System;
using System.Collections;
using System.Collections.Generic;

namespace Vault {
    public class Optional<T> : IEnumerable<T> {
        private T _value;
        private bool isNull;
        
        private Optional(T value, bool isNull = false) {
            this.isNull = isNull;
            _value = value;
        }
        
        public T Get() {
            return _value;
        }
        
        public EmptyOptional IfPresent(Action<T> ifPresent) {
            if (IsPresent()) {
                ifPresent?.Invoke(_value);
                return new EmptyOptional();
            }
            
            return new EmptyOptional(true);
        }
        
        public bool IsPresent() {
            return _value != null && isNull == false;
        }
        
        public T OrElse(T defaultValue) {
            return IsPresent() ? _value : defaultValue;
        }
        
        
        public static Optional<T> Of(T value) {
            return new Optional<T>(value);
        }
        
        
        public static Optional<T> Empty() {
            return new Optional<T>(default, true);
        }
        
        public IEnumerator<T> GetEnumerator() {
            return _value == null ? new List<T>().GetEnumerator() : Lists.Of(_value).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public class EmptyOptional {
            private bool isEmpty;
            
            public EmptyOptional() {
            }
            
            public EmptyOptional(bool isEmpty) {
                this.isEmpty = isEmpty;
            }
            
            public void OrElse(Action orElse) {
                if (isEmpty) {
                    orElse?.Invoke();
                }
            }
        }
    }
}