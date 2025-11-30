using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Constants
{
    public static partial class Constants
    {
        public static class ClaimType
        {
            public const string CLIENT_ID = "clientId";
        }

        public static class CustomJwtClaimTypes
        {
            public const string GPRole = "gpRole";
            public const string EmailConfirmed = "emailConfirmed";
            public const string PhoneNumberConfirmed = "phoneNumberConfirmed";
            public const string IsGoogleLogin = "isgooglelogin";
            public const string Name = "name";
            public const string FullName = "fullName";
        }
    }
}
