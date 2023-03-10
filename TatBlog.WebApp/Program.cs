
using TatBlog.WebApp.Extensions;


var builder = WebApplication.CreateBuilder(args);
{
    builder.ConfigureMvc().ConfigureServices();
}
var app = builder.Build();
{
    app.UseRequestPipeline();
    app.UseBlogRoutes();
    app.UseDataSeeder();
}

app.Run();