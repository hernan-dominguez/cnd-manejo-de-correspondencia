using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Shared.Extensions
{
    public static class DoubleExtensions
    {
        public static object Blank(this double? DoubleValue)
        {
            // Returns a html "blank" character if an empty value
            HtmlString blankSpace = new("&nbsp;");

            return DoubleValue is null ? blankSpace : DoubleValue;
        }
    }
}
