using FTM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.Authen
{
    public partial class SendOTPTracking : BaseEntity
    {
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string RemoteIpAddress { get; set; }
    }
}
