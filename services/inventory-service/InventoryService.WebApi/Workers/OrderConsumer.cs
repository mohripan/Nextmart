using Amazon.SQS;
using Amazon.SQS.Model;
using InventoryService.Domain;
using System.Text.Json;

namespace InventoryService.WebApi.Workers;

public class OrderConsumer : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _config;
    private readonly ILogger<OrderConsumer> _logger;

    public OrderConsumer(IAmazonSQS sqs, IConfiguration config, ILogger<OrderConsumer> logger)
    {
        _sqs = sqs;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _config["SQS_QUEUE_URL"];
        if (string.IsNullOrEmpty(queueUrl))
        {
            _logger.LogError("SQS_QUEUE_URL not configured. OrderConsumer will not start.");
            return;
        }

        _logger.LogInformation("OrderConsumer started, polling {QueueUrl}", queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20
                }, stoppingToken);

                foreach (var message in response.Messages ?? [])
                {
                    try
                    {
                        var order = JsonSerializer.Deserialize<OrderMessage>(message.Body);
                        if (order is not null)
                        {
                            _logger.LogInformation(
                                "Processing order: ProductId={ProductId}, Quantity={Quantity}, Customer={Email}",
                                order.ProductId, order.Quantity, order.CustomerEmail);
                        }

                        await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, stoppingToken);
                        _logger.LogInformation("Message {MessageId} deleted from queue", message.MessageId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process message {MessageId}", message.MessageId);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling SQS, retrying in 5 seconds");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}