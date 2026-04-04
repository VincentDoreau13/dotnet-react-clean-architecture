using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using ShopApi.API.Errors;
using ShopApi.Application.Catalog.Commands.CreateCatalogItem;
using ShopApi.Application.Common.Behaviours;
using ShopApi.Infrastructure.AutofacModules;
using ShopApi.Infrastructure.Data;

// ─── Serilog Bootstrap ───────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Console());

// ─── Autofac ─────────────────────────────────────────────────────────────────
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule<InfrastructureModule>();
});

// ─── Database ───────────────────────────────────────────────────────────────
bool useInMemory = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemory)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("ShopDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            sql => sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorCodesToAdd: null)));
}

// ─── MediatR (scans all handlers in the assembly) ────────────────────────────
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
    configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
    configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
    if (!useInMemory)
        configuration.AddOpenBehavior(typeof(TransactionBehaviour<,>));
});

// ─── FluentValidation ────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// ─── API ─────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Shop API",
        Version = "v1"
    });
    options.DescribeAllParametersInCamelCase();
});

// ─── Health Checks ───────────────────────────────────────────────────────────
var healthChecks = builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database");

if (!useInMemory)
{
    healthChecks.AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "npgsql",
        tags: ["db", "sql"]);
}

// ─── CORS ────────────────────────────────────────────────────────────────────
string[] origins = builder.Configuration
    .GetValue<string>("AllowedOrigins")?
    .Split(";", StringSplitOptions.RemoveEmptyEntries) ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ═══════════════════════════════════════════════════════════════════════════════

var app = builder.Build();

// ─── Database Initialization ─────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (useInMemory)
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

// ─── Middleware Pipeline ──────────────────────────────────────────────────────
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shop API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseExceptionHandler(application => application.UseErrors(app.Environment));
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
