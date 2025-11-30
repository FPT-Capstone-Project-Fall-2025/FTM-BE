using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTM.Domain.Constants.Constants;

namespace FTM.Domain.Enums
{
    public enum NotificationType
    {
        EVENT = Category.FT_NOTIFICATION * 1000 + 1,
        FUND = Category.FT_NOTIFICATION * 1000 + 2,
        INVITE = Category.FT_NOTIFICATION * 1000 + 3,
        RECEIVED = Category.FT_NOTIFICATION * 1000 + 4,
        ACCEPTED = Category.FT_NOTIFICATION * 1000 + 5,
        DECLINE = Category.FT_NOTIFICATION * 1000 + 6,
        SYSTEM = Category.FT_NOTIFICATION * 1000 + 7,
    }
}
