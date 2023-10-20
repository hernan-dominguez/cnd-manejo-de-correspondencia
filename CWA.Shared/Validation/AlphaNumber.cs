using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CWA.Shared.Extensions;

namespace CWA.Shared.Validation
{
    public class AlphaNumberDash : ValidationAttribute
    {
        private readonly string _regex = @"^(?:[a-zA-Z0-9-])*$";

        public override bool IsValid(object value)
        {
            // Accepts only digits and dash characters
            string stringValue = value as string;

            return stringValue.Empty() || Regex.IsMatch(stringValue.Trim(), _regex);
        }
    }
}
