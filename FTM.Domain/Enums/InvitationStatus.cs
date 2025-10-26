using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTM.Domain.Constants.Constants;

namespace FTM.Domain.Enums
{
    public enum InvitationStatus
    {
        PENDING = Category.FT_INVITATION * 1000 + 1,
        ACCEPT = Category.FT_INVITATION * 1000 + 2,
        REJECT = Category.FT_INVITATION * 1000 + 3
    }
}
