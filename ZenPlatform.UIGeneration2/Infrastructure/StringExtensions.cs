using System;
using System.Text;

namespace ZenPlatform.UIGeneration2.Infrastructure {
    public static class StringExtensions {

        public static String CompactString(this String text) {
            var sb = new StringBuilder();
            var wasLastSpace = false;

            for (var x = 0; x < text.Length; x++) {
                var c = text[x];
                if (c != ' ' || !wasLastSpace) {
                    sb.Append(c);
                }

                wasLastSpace = c == ' ';
            }

            sb.Replace(" >", ">");
            return sb.ToString();
        }

        public static String SplitWords(this String text) {
            var sb = new StringBuilder(256);
            var foundUpperCase = false;

            for (var x = 0; x <= text.Length - 1; x++) {
                if (!foundUpperCase && Char.IsUpper(text, x)) {
                    foundUpperCase = true;

                    if (x == 0) {
                        sb.Append(text.Substring(x, 1));
                    } else {
                        sb.Append(" ");
                        sb.Append(text.Substring(x, 1));
                    }

                    continue;
                }

                if (!foundUpperCase) {
                    continue;
                }

                if (Char.IsUpper(text, x)) {
                    sb.Append(" ");
                    sb.Append(text.Substring(x, 1));
                } else if (Char.IsLetterOrDigit(text, x)) {
                    sb.Append(text.Substring(x, 1));
                }
            }

            return sb.ToString();
        }

    }
}
