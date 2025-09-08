using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Vault {
    public static class NativeArrayVault {
        public static List<T> ConvertToList<T>(this NativeArray<T> me) where T : struct {
            var converted = new List<T>();
            foreach (var x1 in me) {
                converted.Add(x1);
            }

            return converted;
        }

        public static T GetLowest<T>(this NativeArray<T> array, Func<T, float> selector) where T : struct {
            var first = selector(array[0]);
            var lowest = array[0];
            foreach (var x1 in array) {
                var current = selector.Invoke(x1);
                if (current < first) {
                    lowest = x1;
                    first = current;
                }
            }

            return lowest;
        }

        public static T GetHighest<T>(this NativeArray<T> array, Func<T, float> selector) where T : struct {
            var first = selector(array[0]);
            var lowest = array[0];
            foreach (var x1 in array) {
                var current = selector.Invoke(x1);
                if (current > first) {
                    lowest = x1;
                    first = current;
                }
            }

            return lowest;
        }
    }
}