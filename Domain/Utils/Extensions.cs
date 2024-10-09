using System.Runtime.CompilerServices;

namespace Domain.Utils {
    public static class Extensions {
        public static bool HasValue(this object? value) {
            return value != null;
        }
        public static bool HasValue(this string? value) {
            return !string.IsNullOrEmpty(value);
        }
        public static bool IsEmpty<T>(this List<T> list) {
            return list.Count == 0;
        }
    }
}
