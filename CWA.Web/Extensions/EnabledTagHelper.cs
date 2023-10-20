using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Extensions
{
    [HtmlTargetElement(Attributes = "cwa-enabled")] 
    public class EnabledTagHelper : TagHelper
    {
        public object CwaEnabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool disable = false;

            output.Attributes.RemoveAll("cwa-enabled");
            TagHelperAttribute disabled = new("disabled");

            if (CwaEnabled is null) disable = true;
            
            if (CwaEnabled is bool asBool) disable = !asBool;

            if (CwaEnabled is string asString) disable = asString.Empty();

            if (disable) output.Attributes.SetAttribute(disabled);
        }
    }
}