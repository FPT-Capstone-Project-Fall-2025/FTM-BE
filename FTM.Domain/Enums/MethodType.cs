using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTM.Domain.Constants.Constants;

namespace FTM.Domain.Enums
{
    public enum MethodType
    {
        VIEW = Category.FT_AUTHORIZATION_METHOD * 1000 + 1,
        ADD = Category.FT_AUTHORIZATION_METHOD * 1000 + 2,
        UPDATE = Category.FT_AUTHORIZATION_METHOD * 1000 + 3,
        DELETE = Category.FT_AUTHORIZATION_METHOD * 1000 + 4,
        ALL = Category.FT_AUTHORIZATION_METHOD * 1000 + 5,
    }
}
