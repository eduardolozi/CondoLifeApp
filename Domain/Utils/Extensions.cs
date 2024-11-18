using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;

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

        public static bool ContainsSignalRHub(this string path)
        {
            var hubPaths = new HashSet<string>
            {
                "notificationHub",
            };
            
            return hubPaths.Contains(path);
        }
    }
}
