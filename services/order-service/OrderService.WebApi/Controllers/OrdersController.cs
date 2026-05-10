using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain;
using System.Text.Json;

namespace OrderService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _config;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IAmazonSQS sqs, IConfiguration config, ILogger<OrdersController> logger)
    {
        _sqs = sqs;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var queueUrl = _config["SQS_QUEUE_URL"];
        if (string.IsNullOrEmpty(queueUrl))
            return StatusCode(500, "SQS_QUEUE_URL not configured");

        var message = JsonSerializer.Serialize(request);

        await _sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = message
        });

        _logger.LogInformation("Order published to SQS for product {ProductId}", request.ProductId);

        return Accepted(new { message = "Order received", productId = request.ProductId });
    }
}