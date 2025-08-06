using Microsoft.EntityFrameworkCore;
using SocietyMng.Data;
using SocietyMng.Extensions;

var builder = WebApplication.CreateBuilder(args);

//extensions services' builder
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureAppSettings(builder.Configuration);
builder.Services.ConfigureSession();
builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthorization();
builder.Services.ConfigureControllersWithViews();
builder.Services.ConfigureHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddMvc().AddSessionStateTempDataProvider();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database migration failed.");
    }
}
app.ConfigureMiddleware();
app.UseSession();

app.Run();


