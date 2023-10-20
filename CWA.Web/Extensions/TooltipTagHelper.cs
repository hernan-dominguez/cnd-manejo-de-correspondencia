using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CWA.Web.Extensions
{
    [HtmlTargetElement(Attributes = "cwa-tooltip")]
    public class TooltipTagHelper : TagHelper
    {
        public string CwaTooltip { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("cwa-tooltip");
            output.Attributes.Add("data-bs-toggle", "tooltip");
            output.Attributes.Add("data-bs-placement", "top");
            output.Attributes.Add("data-bs-html", "true");
            output.Attributes.Add("title", $"{CwaTooltip}");
        }
    }
}
/*
 
    <button type="button" class="btn btn-secondary" data-bs-toggle="tooltip" data-bs-placement="top" title="Tooltip on top">
  Tooltip on top
</button>

<button type="button" class="btn btn-secondary" data-bs-toggle="tooltip" data-bs-html="true" title="<em>Tooltip</em> <u>with</u> <b>HTML</b>">
  Tooltip with HTML
</button>

 */