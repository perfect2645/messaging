# Webapi Messaging Tool

## .net WebApi
### builder.Services.AddSwaggerGen
- 这是用于配置 Swagger 文档生成的 服务注册 方法。
- 它会在依赖注入容器中添加 Swagger 相关的服务，并允许你自定义 Swagger 文档的生成方式（如 API 的元数据、描述、版本控制等）。

### app.UseSwagger
- 这是用于在 ASP.NET Core 应用程序中启用 Swagger 中间件的方法,它定义 Swagger UI 的展示方式（如文档终结点、页面样式等）。
- 它会将 Swagger 文档生成的结果暴露为一个 HTTP 端点，通常是 `/swagger/v1/swagger.json`。


## Package Overview
### Package - Api Versioning
[ApiVersioning](https://github.com/dotnet/aspnet-api-versioning)

### Package - Log4Net
[Log4Net](https://logging.apache.org/log4net/)