using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace webapi_messaging.Configurations.Swagger
{
    public class VersionControlParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters
                .FirstOrDefault(p => p.Name == "api-version");
            if (versionParameter == null)
            {
                return;
            }

            // Remove the version parameter from the operation
            operation.Parameters.Remove(versionParameter);
        }
    }
}
