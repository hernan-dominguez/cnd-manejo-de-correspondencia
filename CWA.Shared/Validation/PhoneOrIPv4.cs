using CWA.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CWA.Shared.Validation
{
    public enum PhoneOrIPv4Formats
    {
        Phone, IPv4, PhoneOrIPv4
    }

    public class PhoneOrIPv4 : ValidationAttribute
    {
        public PhoneOrIPv4Formats AccessType { get; set; } = PhoneOrIPv4Formats.Phone;

        private readonly string _phone = @"^([1-9]{1}[0-9]{2,3}[-][0-9]{4})$";
        private readonly string _extension = @"^([1-9]{1}[0-9]{2,3}[-][0-9]{4}\sext.\s[0-9]{1,5})$";
        private readonly string _ipv4 = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?:([:]{1}[1-9]{1}[0-9]{0,5})?)$";

        public override bool IsValid(object value)
        {
            // Accept commonly recognized phone numbers and ip address with port numbers
            // Notes:
            // - Phone number: 1000-0000 to 9999-9999
            // - Phone number with extension: ####-#### ext. [1 to 99999]
            // - IP address: 000.000.000.000
            // - IP adddress with port: 000.000.000.000:[1 to 99999]

            string stringValue = value as string;

            if (!stringValue.Empty())
            {
                stringValue = stringValue.Trim().ToLower();

                switch (AccessType)
                {
                    case PhoneOrIPv4Formats.Phone:
                        return Regex.IsMatch(stringValue, _phone) || Regex.IsMatch(stringValue, _extension);

                    case PhoneOrIPv4Formats.IPv4:
                        return Regex.IsMatch(stringValue, _ipv4);

                    case PhoneOrIPv4Formats.PhoneOrIPv4:
                        return Regex.IsMatch(stringValue, _phone) || Regex.IsMatch(stringValue, _extension) || Regex.IsMatch(stringValue, _ipv4);
                }
            }

            return true;
        }
    }
}
