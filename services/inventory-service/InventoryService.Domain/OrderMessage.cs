namespace InventoryService.Domain;

public class OrderMessage
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
}