namespace ECommerce.UseCases.Basket.DTOs;

using BasketEntity = Domain.Entities.Basket;

public sealed record BasketItemResponse(Guid ProductId, string ProductName,
    string PictureUrl, decimal UnitPrice, int Quantity, decimal LineTotal);
public sealed record GetBasketResponse(Guid BuyerId, IReadOnlyList<BasketItemResponse> Items,
    int TotalItems, decimal SubTotal)
{
    public static GetBasketResponse From(BasketEntity basket) 
        => new(basket.BuyerId,basket.Items.Select(item => new BasketItemResponse(item.ProductId, item.ProductName,
            item.PictureUrl, item.UnitPrice, item.Quantity, item.LineTotalPrice)).ToList() ,
            basket.TotalItems, basket.SubTotal);

}
public sealed record AddItemRequest(Guid ProductId, int Quantity);
public sealed record UpdateQuantityRequest(int Quantity);
