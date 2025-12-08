using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace FTM.API.Helpers
{
    public class FTAuthorizationHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // --- Check FTAuthorize attribute (Action level)
            var ftAuthorize = context.MethodInfo.GetCustomAttribute<FTAuthorizeAttribute>();

            // Check FTAuthorize on controller
            ftAuthorize ??= context.MethodInfo.DeclaringType?
                .GetCustomAttribute<FTAuthorizeAttribute>();

            // --- Check FTAuthorizeOwner attribute
            var ftOwner = context.MethodInfo.GetCustomAttribute<FTAuthorizeOwnerAttribute>()
                       ?? context.MethodInfo.DeclaringType?
                           .GetCustomAttribute<FTAuthorizeOwnerAttribute>();

            // If NEITHER attribute exists → NO HEADER
            if (ftAuthorize == null && ftOwner == null)
                return;

            // Initialize Swagger parameters
            operation.Parameters ??= new List<OpenApiParameter>();

            // Add your custom header (example name)
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Ftid",
                In = ParameterLocation.Header,
                Description = "Required for FTAuthorization",
                Required = true
            });
        }
    }
}
