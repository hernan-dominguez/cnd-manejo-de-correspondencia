﻿using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Shared.Extensions
{
    public static class IntExtensions
    {
        public static object Blank(this int? IntValue)
        {
            // Returns a html "blank" character if an empty value
            HtmlString blankSpace = new("&nbsp;");

            return IntValue is null ? blankSpace : IntValue;
        }
    }
}
