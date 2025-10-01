﻿using FTM.Infrastructure.Repositories.Interface;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static FTM.Domain.Constants.Constants;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class CurrentUserResolver : ICurrentUserResolver
    {
        public CurrentUserResolver(IHttpContextAccessor httpContextAccessor)
        {
            var currentUser = httpContextAccessor.HttpContext.User;
            RemoteIpAddress = httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            if (currentUser?.Identity?.IsAuthenticated == true)
            {
                // UserId (sub or NameIdentifier)
                var userId = currentUser.FindFirst(JwtClaimTypes.Subject)?.Value
                          ?? currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    UserId = new Guid(userId);
                }

                // Username (Name or custom)
                Username = currentUser.FindFirst(JwtClaimTypes.Name)?.Value
                        ?? currentUser.FindFirst(ClaimTypes.Name)?.Value;

                // Email
                Email = currentUser.FindFirst(JwtClaimTypes.Email)?.Value
                     ?? currentUser.FindFirst(ClaimTypes.Email)?.Value;

                // Role(s)
                Role = string.Join(',', currentUser.FindAll(ClaimTypes.Role).Select(x => x.Value));

                // Full Name (custom claim)
                Name = currentUser.FindFirst(CustomJwtClaimTypes.FullName)?.Value;
            }
        }

        public Guid UserId { get; }
        public string Username { get; }
        public string Email { get; }
        public string Role { get; }
        public string RemoteIpAddress { get; }
        public string Name { get; }
    }
}
