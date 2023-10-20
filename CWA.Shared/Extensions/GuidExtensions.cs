using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Shared.Extensions
{
    public static class GuidExtensions
    {
        public static string GetString(this Guid GuidValue, int StartPosition, int Length, bool UpperCase = false)
        {
            // Return a customized string representation of this Guid value
            string retValue = UpperCase ? GuidValue.ToString().Replace("-", "").ToUpper() : GuidValue.ToString().Replace("-", "");

            if (Length <= 0) return string.Empty;

            Length = (Length <= retValue.Length - StartPosition) ? Length : retValue.Length - StartPosition;

            return retValue.Substring(StartPosition, Length);
        }
    }
}
