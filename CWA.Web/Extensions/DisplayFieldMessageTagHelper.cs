using CWA.Shared.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Extensions
{
    [HtmlTargetElement("span", Attributes = "cwa-for")]
    public class DisplayFieldMessageTagHelper : TagHelper
    {
        [HtmlAttributeName("cwa-for")]
        public ModelExpression ModelElement { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("cwa-for");

            if (ModelElement.Name.Contains("."))
            {
                output.Attributes.Add("id", ModelElement.Name.Replace(".", "-"));
            }
            else
            {
                output.Attributes.Add("id", $"Model-{ModelElement.Name}");
            }

            output.Content.SetHtmlContent("&nbsp;");
        }
    }
}
