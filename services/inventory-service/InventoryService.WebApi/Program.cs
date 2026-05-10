using Amazon.SQS;
using InventoryService.WebApi.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddHostedService<OrderConsumer>();

var app = builder.Build();

app.MapGet("/healthz", () => Results.Ok("healthy"));
app.UseAuthorization();
app.MapControllers();
app.Run();