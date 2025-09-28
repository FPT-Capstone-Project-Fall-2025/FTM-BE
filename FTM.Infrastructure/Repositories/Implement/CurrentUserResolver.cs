using static FTM.Domain.Constants.Constants;
using IdentityModel;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class CurrentUserResolver : ICurrentUserResolver
    {

        public CurrentUserResolver(IHttpContextAccessor httpContextAccessor)
        {
            var currentUser = httpContextAccessor.HttpContext.User;

            RemoteIpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (currentUser != null)
            {
                var userId = currentUser?.FindFirst(JwtClaimTypes.Subject)?.Value;
                if (userId != null)
                {
                    UserId = new Guid(userId);
                }
                Username = currentUser?.FindFirst(JwtClaimTypes.Name)?.Value;
                Email = currentUser?.FindFirst(JwtClaimTypes.Email)?.Value;
                Role = String.Join(',', currentUser?.FindAll(JwtClaimTypes.Role).Select(x => x.Value));
                Name = currentUser?.FindFirst(CustomJwtClaimTypes.FullName)?.Value;
            }
        }

        public Guid UserId { get; }

        public string Username { get; }

        public string Email { get; }

        public string Role { get; }
        public string RemoteIpAddress { get; set; }

        public string Name { get; }
    }
}
