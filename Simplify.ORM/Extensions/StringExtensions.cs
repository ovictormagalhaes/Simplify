using System.Globalization;
using System.Text;

namespace Simplify.ORM.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var parts = str.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return str;

            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                if (sb.Length == 0)
                {
                    sb.Append(part.Substring(0, 1).ToLower(CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(part.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture));
                }
                sb.Append(part.Substring(1).ToLower(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        sb.Append('_');
                    }
                    sb.Append(char.ToLower(c, CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var parts = str.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return str;

            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                sb.Append(part.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture));
                sb.Append(part.Substring(1).ToLower(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }
    }
}
