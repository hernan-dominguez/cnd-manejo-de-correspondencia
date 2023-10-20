using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Extensions
{
    [HtmlTargetElement(Attributes = "cwa-visible")]
    public class VisibleTagHelper : TagHelper
    {
        public object CwaVisible { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool hide = false;

            output.Attributes.RemoveAll("cwa-visible");

            if (CwaVisible is null) hide = true;

            if (CwaVisible is bool asBool) hide = !asBool;

            if (CwaVisible is string asString) hide = asString.Empty();

            if (hide) output.SuppressOutput();
        }
    }
}