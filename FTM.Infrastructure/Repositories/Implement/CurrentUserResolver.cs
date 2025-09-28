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
            RemoteIpAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
            
            var currentUser = httpContextAccessor.HttpContext?.User;
            if (currentUser != null && currentUser.Identity.IsAuthenticated)
            {
                // Try multiple claim types for UserId
                var userId = currentUser.FindFirst(JwtClaimTypes.Subject)?.Value 
                           ?? currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? currentUser.FindFirst("sub")?.Value;
                           
                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var parsedUserId))
                {
                    UserId = parsedUserId;
                }

                Username = currentUser.FindFirst(JwtClaimTypes.Name)?.Value 
                        ?? currentUser.FindFirst(ClaimTypes.Name)?.Value;
                        
                Email = currentUser.FindFirst(JwtClaimTypes.Email)?.Value 
                     ?? currentUser.FindFirst(ClaimTypes.Email)?.Value;
                     
                Role = String.Join(',', currentUser.FindAll(JwtClaimTypes.Role)
                                                  .Concat(currentUser.FindAll(ClaimTypes.Role))
                                                  .Select(x => x.Value)
                                                  .Distinct());
                                                  
                Name = currentUser.FindFirst(CustomJwtClaimTypes.FullName)?.Value;
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
