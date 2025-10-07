using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.Authen
{
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Nickname { get; set; }
        public DateTime? Birthday { get; set; }
        public string Job { get; set; }
        public int? Gender { get; set; }
        public string Picture { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        
        // Province and Ward information
        public ProvinceInfo Province { get; set; }
        public WardInfo Ward { get; set; }
        
        // Role information
        public List<string> Roles { get; set; }
        
        // Account dates
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }

    public class ProvinceInfo
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameWithType { get; set; }
    }

    public class WardInfo
    {
        public Guid? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameWithType { get; set; }
        public string Path { get; set; }
        public string PathWithType { get; set; }
    }
}