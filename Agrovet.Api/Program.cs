using Agrovet.Application.Interfaces.Repositories;
using Agrovet.Infrastructure.Data;
using Agrovet.Infrastructure.Data.Seed;
using Agrovet.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

try
{
    // Configure Serilog before building the host
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.File(
            path: "Logs/agrovet-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            path: "Logs/agrovet-debug-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 3,
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    // Replace default logging with Serilog
    builder.Host.UseSerilog();

    // Add services
    builder.Services.AddDbContext<AgrovetDbContext>(options =>
    {
        options.UseSqlite("Data Source=agrovet.db");
    });

    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Custom request logging - only log meaningful HTTP interactions
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex != null) return Serilog.Events.LogEventLevel.Error;
            if (httpContext.Response.StatusCode >= 500) return Serilog.Events.LogEventLevel.Error;
            if (httpContext.Response.StatusCode >= 400) return Serilog.Events.LogEventLevel.Warning;
            return Serilog.Events.LogEventLevel.Information;
        };
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // Apply migrations
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AgrovetDbContext>();
        db.Database.Migrate();
        Log.Information("Database migrations completed");
        
        try
        {
            await DbSeeder.SeedAsync(db);
            Log.Information("Database seeding completed");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error seeding database");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}