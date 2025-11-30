using System.Text.RegularExpressions;

namespace FTM.Domain.Helpers
{
    public static class StringHelper
    {
        public static string RemoveExtraWhiteSpace(this string input)
        {
            return Regex.Replace((input ?? "").Trim(), @"\s\s+", " ");
        }

        public static string FormatPhoneNumber(this string input)
        {
            if ((input.StartsWith("0") && input.Length != 10) || (input.StartsWith("+") && input.Length != 12))
            {
                throw new ArgumentException("Số điện thoại không hợp lệ.");
            }
            var result = Regex.Replace((input ?? "").Trim(), @"\s\s+", " ");
            result = result.Replace("+0", "");
            result = result.Replace("+", "");
            result = result.Replace(" ", "");
            result = result.Replace("+", "");

            if (result.StartsWith("0"))
            {
                return string.Concat("84", result.AsSpan(1));
            }

            return result;
        }

        public static bool IsEqualOrdinalIgnoreCase(this string input, string compare)
        {
            input = input.RemoveExtraWhiteSpace();
            compare = compare.RemoveExtraWhiteSpace();
            return input.Equals(compare, StringComparison.OrdinalIgnoreCase);
        }
    }
}
