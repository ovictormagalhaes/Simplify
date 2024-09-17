using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Simplify.ORM.Generator
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var temp = string.Copy(str);

            if (!temp.Contains(" ") && !temp.Contains("_") && !temp.Contains("-"))
            {
                if (temp.All(char.IsUpper))
                {
                    return char.ToLower(temp[0], CultureInfo.InvariantCulture) + temp.Substring(1).ToLower(CultureInfo.InvariantCulture);
                }
                return char.ToLower(temp[0], CultureInfo.InvariantCulture) + temp.Substring(1);
            }

            var parts = temp.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return string.Copy(temp);

            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                if (sb.Length == 0)
                    sb.Append(part.Substring(0, 1).ToLower(CultureInfo.InvariantCulture));
                else
                    sb.Append(part.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture));
                sb.Append(part.Substring(1).ToLower(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var temp = string.Copy(str).Trim();

            var sb = new StringBuilder();
            bool wasPreviousUpper = false;
            bool wasPreviousUnderscore = false;

            for (int i = 0; i < temp.Length; i++)
            {
                char c = temp[i];

                if (c == ' ' || c == '-')
                {
                    if (!wasPreviousUnderscore)
                    {
                        sb.Append('_');
                        wasPreviousUnderscore = true;
                    }
                    wasPreviousUpper = false;
                }
                else if (char.IsUpper(c))
                {
                    if (i > 0 && !wasPreviousUpper && temp[i - 1] != ' ' && temp[i - 1] != '_' && temp[i - 1] != '-')
                        sb.Append('_');

                    sb.Append(char.ToLower(c));
                    wasPreviousUpper = true;
                    wasPreviousUnderscore = false;
                }
                else
                {
                    sb.Append(c);
                    wasPreviousUpper = false;
                    wasPreviousUnderscore = false;
                }
            }

            return sb.ToString();
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var temp = string.Copy(str);

            var parts = temp.Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return str;

            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                sb.Append(char.ToUpper(part[0], CultureInfo.InvariantCulture));
                sb.Append(part.Substring(1));
            }

            return sb.ToString();
        }
    }

}
