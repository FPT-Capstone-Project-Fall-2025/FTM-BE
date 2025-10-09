using FTM.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<ApplicationUserRole?> GetUserRoleWithRelated(Guid userId, string roleName);
    }
}