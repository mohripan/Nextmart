using Amazon.SQS;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAWSService<IAmazonSQS>();

var app = builder.Build();

app.MapGet("/healthz", () => Results.Ok("healthy"));
app.UseAuthorization();
app.MapControllers();
app.Run();