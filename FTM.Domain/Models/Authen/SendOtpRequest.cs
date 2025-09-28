using FTM.Domain.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Models.Authen
{
    public class SendOtpRequest
    {
        [EmailAddress]
        [JsonConverter(typeof(EmailSensitive))]
        public string Email { get; set; }

        [Phone]
        [JsonConverter(typeof(PhoneSensitive))]
        public string PhoneNumber { get; set; }
        public string RemoteIpAddress { get; set; }
        public string Content { get; set; }
    }
}
