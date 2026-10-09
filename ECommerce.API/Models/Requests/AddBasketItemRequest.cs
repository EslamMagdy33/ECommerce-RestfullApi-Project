
namespace ECommerce.API.Models.Requests;

public sealed class AddBasketItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
