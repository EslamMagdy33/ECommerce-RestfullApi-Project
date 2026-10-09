using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models.Requests;

public sealed class UpdateBasketItemQuantityRequest
{
    private const int MinQuantity = 1;
    private const int MaxQuantity = 100;

    [Range(MinQuantity, MaxQuantity)]
    public int Quantity { get; init; }
}
