using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RemoteGallery.Validations
{
    internal class IpAddressRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return IPAddress.TryParse(value as string, out _) 
                ? ValidationResult.ValidResult : new ValidationResult(false, "Invalid IP address");
        }
    }
}
