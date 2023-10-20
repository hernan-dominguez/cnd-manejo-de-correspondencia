using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string GetNormalized(this string StringValue)
        {
            // Remove border emptiness and set all lowercase
            StringValue = StringValue.Trim().ToLower();

            // Remove extra inner whitespaces
            while (StringValue.Contains("  ")) StringValue = StringValue.Replace("  ", " ");

            // De-accute vowels and replace 'ñ with 'n
            StringValue = StringValue
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("ñ", "n");

            return StringValue;
        }

        public static string GetSafeName(this string StringValue)
        {
            // Accepts alphas and underscore, convert anything else to underscore character
            string allowedChars = "abcdefghijklmnopqrstuvwxyz1234567890_";
            char[] valueChars = StringValue.GetNormalized().ToCharArray();

            // Strip and compose
            string retValue = "";

            for (int charIndex = 0; charIndex < valueChars.Length; charIndex++)
            {
                retValue = String.Concat(retValue, allowedChars.Contains(valueChars[charIndex].ToString()) ? valueChars[charIndex].ToString() : "_");
            }

            return retValue;
        }

        public static List<string> SplitList(this string StringValue, char Separator)
        {
            // A shortened version
            return StringValue.Split(new char[] { Separator }).ToList();
        }

        public static object Blank(this string StringValue, string NewText = "")
        {
            // Returns a html "blank" character if an empty string
            HtmlString blankSpace = NewText.Empty() ? new("&nbsp;") : new(NewText);

            return StringValue is null ? blankSpace : StringValue;
        }

        public static bool Empty(this string StringValue)
        {
            // A shortened version 
            return String.IsNullOrEmpty(StringValue);
        }

        public static bool HasValue(this string StringValue)
        {
            // A shortened version 
            return String.IsNullOrEmpty(StringValue);
        }
    }    
}
