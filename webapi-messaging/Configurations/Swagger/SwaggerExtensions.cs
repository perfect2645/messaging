using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace webapi_messaging.Configurations.Swagger
{
    public static class SwaggerExtensions
    {
        public static void AddSwaggerGenExt(this WebApplicationBuilder builder)
        {
            builder.ConfigSwaggerVersioning();
            builder.Services.AddSwaggerGen(options =>
            {
                #region Attach Swagger XML Comments

                var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                // Add the XML comments file for Swagger
                options.IncludeXmlComments(xmlPath, true);
                options.OrderActionsBy(o => o.RelativePath);

                #endregion Attach Swagger XML Comments
            });
        }

        private static void ConfigSwaggerVersioning(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>,
                ConfigureSwaggerOptions>();
            builder.Services.AddSwaggerGen(options =>
            {
                #region Switch API versions by Asp.Versioning.Mvc

                //add a custom operation filter to set the default values
                //options.OperationFilter<SwaggerDefaultValues>(); TODO
                options.DescribeAllParametersInCamelCase();
                options.OperationFilter<VersionControlParameter>();

                #endregion Switch API versions by Asp.Versioning.Mvc
            });
        }

        public static void UseSwaggerExt(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions.Reverse())
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"{app.Environment.ApplicationName}{description.GroupName.ToUpperInvariant}");
                }
            });
        }
    }
}
