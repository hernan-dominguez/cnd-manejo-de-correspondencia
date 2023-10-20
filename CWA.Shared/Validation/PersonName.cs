using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CWA.Shared.Extensions;

namespace CWA.Shared.Validation
{
    public class PersonName : ValidationAttribute
    {
        private readonly string _regex = @"^(?:[a-zA-Z.ñÑáéíóúÁÉÍÓÚ\s])*$";

        public override bool IsValid(object value)
        {
            // Accepts common characters for a person's name, period and whitespaces
            string stringValue = value as string;

            return stringValue.Empty() || Regex.IsMatch(stringValue.Trim(), _regex);
        }
    }
}
