using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Enums
{
    public enum AccountStatus
    {
        [EnumMember(Value = "MaximumFail")]
        MaximumFail = -1,
        Activated = 2,
        DoNotConfirmedEmail = 3,
        DoNotConfirmPhoneNumber = 4,
        DoNotConfirmedPhoneNumberRequired = 5
    }
}
