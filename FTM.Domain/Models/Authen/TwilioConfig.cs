using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Models.Authen
{
    public class TwilioConfig
    {
        public string AccountSID { get; set; }
        public string AuthToken { get; set; }
        public string PhoneNumberFrom { get; set; }
    }
}
