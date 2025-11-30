using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTM.Domain.Constants.Constants;

namespace FTM.Domain.Enums
{
    public enum FeatureType
    {
        MEMBER = Category.FT_AUTHORIZATION_FEATURE * 1000 + 1,
        EVENT = Category.FT_AUTHORIZATION_FEATURE * 1000 + 2,
        FUND = Category.FT_AUTHORIZATION_FEATURE * 1000 + 3,
        ALL = Category.FT_AUTHORIZATION_FEATURE * 1000 + 4
    }
}
