using System.Text;

namespace HttpProxyGenerator.Common.Extensions
{
    internal static class StringExtensions
    {
        public static string ToKebabCase(this string source)
        {
            // Implemented according to https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/capitalization-conventions
            if (source is null)
            {
                return null;
            }

            if (source.Length == 0)
            {
                return string.Empty;
            }

            const char hyphen = '-';

            var builder = new StringBuilder();

            for (var i = 0; i < source.Length; i++)
            {
                // if current char is already lowercase
                if (char.IsLower(source[i]))
                {
                    builder.Append(source[i]);
                }

                // if current char is the first char
                else if (i == 0) 
                {
                    builder.Append(char.ToLower(source[i]));
                }

                // if current char is a number and the previous is not
                else if (char.IsDigit(source[i]) && !char.IsDigit(source[i - 1]))
                {
                    builder.Append(hyphen);
                    builder.Append(source[i]);
                }

                // if current char is a number and previous is
                else if (char.IsDigit(source[i]))
                {
                    builder.Append(source[i]);
                }

                // if current char is upper and previous char is lower
                else if (char.IsLower(source[i - 1])) 
                {
                    builder.Append(hyphen);
                    builder.Append(char.ToLower(source[i]));
                }

                // if current char is upper and next char doesn't exist or is upper
                else if (i + 1 == source.Length || char.IsUpper(source[i + 1])) 
                {
                    builder.Append(char.ToLower(source[i]));
                }

                // if current char is upper and next char is lower
                else
                {
                    builder.Append(hyphen);
                    builder.Append(char.ToLower(source[i]));
                }
            }
            return builder.ToString();
        }
    }
}
