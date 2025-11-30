using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace FTM.API.Middleware
{
    public class FTAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public FTAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IFTAuthorizationService authService)
        {
            var endpoint = context.GetEndpoint();
            var attr = endpoint?.Metadata.GetMetadata<FTAuthorizeAttribute>();
            var ownerAttr = endpoint?.Metadata.GetMetadata<FTAuthorizeOwnerAttribute>();

            if (attr == null && ownerAttr == null)
            {
                await _next(context);
                return;
            }

            // Get user from JWT
            if (!Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            {
                await WriteError(context, HttpStatusCode.Unauthorized, "Không có quyền");
                return;
            }

            // Get FamilyTree Id from Header
            if (!Guid.TryParse(context.Request.Headers["X-Ftid"], out var ftId))
            {
                await WriteError(context, HttpStatusCode.BadRequest, "Thiếu hoặc sai X-Ftid header");
                return;
            }

            // Check membership
            if (!await authService.IsAccessedToFamilyTreeAsync(ftId, userId))
            {
                await WriteError(context, HttpStatusCode.Forbidden, "Bạn không phải là thành viên của cây gia phả này");
                return;
            }
            
            var isOwner = await authService.IsOwnerAsync(ftId, userId);
            var isGuest = await authService.IsGuestAsync(ftId, userId);

            // If the user is owner
            if (isOwner)
            {
                await _next(context);
                return;
            }

            // If only owner allowed
            if (ownerAttr != null)
            {
                await WriteError(context, HttpStatusCode.Forbidden, "Chỉ người sở hữu cây gia phả mới truy cập được chức năng này");
                return;
            }

            // If the user is guest
            if (isGuest && 
                ((attr.Feature == FeatureType.MEMBER && attr.Method == MethodType.VIEW) 
                 || (attr.Feature == FeatureType.EVENT && attr.Method == MethodType.VIEW)))
            {
                await _next(context);
                return;
            }

            // Permission based authorization
            if (!await authService.HasPermissionAsync(ftId, userId, attr.Feature, attr.Method))
            {
                await WriteError(context, HttpStatusCode.Forbidden, "Bạn không có quyền");
                return;
            }

            await _next(context);
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode code, string message)
        {
            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new ApiError(message, code));
            await context.Response.WriteAsync(result);
        }
    }
}
