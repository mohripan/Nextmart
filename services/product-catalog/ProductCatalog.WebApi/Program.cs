using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductCatalog.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Database — connection string comes from env var injected by K8s secret
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// OpenTelemetry — traces go to ADOT collector (we'll set this up in Phase 7) 
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("product-catalog"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
                ?? "http://adot-collector:4317");
        }));

builder.Services.AddControllers();

var app = builder.Build();

// Auto-migrate on startup — fine for learning, not for production
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();
}

app.MapGet("/healthz", () => Results.Ok("healthy"));

app.MapControllers();
app.Run();