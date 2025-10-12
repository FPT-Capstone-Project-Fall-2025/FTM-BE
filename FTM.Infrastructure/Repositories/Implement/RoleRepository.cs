using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppIdentityDbContext _context;

        public RoleRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationRole> GetRoleByName(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role '{roleName}' not found.");
            }
            return role;
        }
    }
}