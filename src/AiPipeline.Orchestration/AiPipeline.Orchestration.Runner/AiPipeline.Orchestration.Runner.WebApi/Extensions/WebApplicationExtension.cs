using Microsoft.Extensions.FileProviders;

namespace AiPipeline.Orchestration.Runner.WebApi.Extensions;

public static class WebApplicationExtension
{
    public static WebApplication AddSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }

    private static void AddRootDocsRedirect(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var isRoot = context.Request.Path == "/";
            if (isRoot)
            {
                context.Response.Redirect("/docs");
                return;
            }

            await next();
        });
    }

    public static WebApplication AddMkDocs(this WebApplication app)
    {
        app.UseDefaultFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "wwwroot", "docs")),
            RequestPath = "/docs",
        });

        AddRootDocsRedirect(app);

        return app;
    }
}