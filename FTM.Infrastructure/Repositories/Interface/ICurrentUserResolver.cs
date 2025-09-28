using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface ICurrentUserResolver
    {
        public Guid UserId { get; }

        public string Username { get; }

        public string Name { get; }

        string Email { get; }

        public string Role { get; }

        string RemoteIpAddress { get; }

       // Task<List<UserClaimDto>> GetGPRoleAsync();
    }
}
