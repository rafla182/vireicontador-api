using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VireiContador.Infra.Extensions
{
    public static class StringExtensions
    {
        private static readonly HashSet<char> DefaultNonWordCharacters  = new HashSet<char> { ',', '.', ':', ';' };

        public static string Resumir(this string value, int length, HashSet<char> nonWordCharacters = null)
        {
            if (nonWordCharacters == null)
            {
                nonWordCharacters = DefaultNonWordCharacters;
            }

            if (length >= value.Length)
            {
                return value;
            }
            int end = length;

            for (int i = end; i > 0; i--)
            {
                if (value[i].IsWhitespace())
                {
                    break;
                }

                if (nonWordCharacters.Contains(value[i])
                    && (value.Length == i + 1 || value[i + 1] == ' '))
                {
                    break;
                }
                end--;
            }

            if (end == 0)
            {
                end = length;
            }

            return $"{value.Substring(0, end)}...";
        }

        private static bool IsWhitespace(this char character)
        {
            return character == ' ' || character == 'n' || character == 't';
        }

        public static string GerarUrl(this string phrase)
        {
            string str = RemoverAcentos(phrase).ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            // str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoverAcentos(this string txt)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string RetornarDominio(this string url)
        {
            return Regex.Replace(url, @"^([a-zA-Z]+:\/\/)?([^\/]+)\/.*?$", "$2");
        }
    }
}
