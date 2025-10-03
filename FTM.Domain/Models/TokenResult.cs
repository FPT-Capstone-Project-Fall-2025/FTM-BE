using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FTM.Domain.Models
{
    public class TokenResult
    {
        /// <summary>
        /// User id
        /// </summary>
        //public Guid UserId { get; set; }

        ///// <summary>
        ///// Username (Phone number)
        ///// </summary>
        //public string Username { get; set; }

        ///// <summary>
        ///// Phone number
        ///// </summary>
        //public string Phone { get; set; }

        ///// <summary>
        ///// Email
        ///// </summary>
        //public string Email { get; set; }
        //public string Address { get; set; }
        //public IList<string> Roles { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        //[JsonConverter(typeof(JsonStringEnumConverter))]
        //public AccountStatus AccountStatus { get; set; }
        //public string Picture { get; set; }
        //public string Fullname { get; set; }

        //public List<UserClaimDto> UserClaims { get; set; }
    }
}
