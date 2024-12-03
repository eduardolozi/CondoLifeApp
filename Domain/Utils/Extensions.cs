using System.Globalization;
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

        public static DateTime FormatDateToBrazilianPattern(this DateTime date)
        {
            var formatedDate = date.ToString("MM/dd/yyyy HH:mm:ss");
            return DateTime.Parse(formatedDate, new CultureInfo("pt-br"));
        }
    }
}
