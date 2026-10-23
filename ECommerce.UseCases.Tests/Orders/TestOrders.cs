using ECommerce.Domain.Entities;

namespace ECommerce.UseCases.Tests.Orders;

internal static class TestOrders
{
    public static Order Pending(Guid userId,string? paymentIntentId = null)
    {
        var deliveryMethod = DeliveryMethod.Create(Guid.NewGuid(), "Standard", 20m, "3 - 5 days").Value!;

        var address= UserAddress.Create(Guid.NewGuid(),userId,"Home", "Ada", "Lovelace", "0100000000",
            "Egypt", "Giza", "1 Pyramids St", "12511").Value!;

        var lines=new List<(Guid ProductId, string ProductName, string PictureUrl , decimal Price, int Quantity)>
        {
            (Guid.NewGuid(), "Phone", "https://cdn/phone.png", 100m, 2),
        };

        var order = Order.Create(Guid.NewGuid(), userId,deliveryMethod ,address , lines).Value!;
        if (!string.IsNullOrWhiteSpace(paymentIntentId))
            order.AttachPaymentIntent(paymentIntentId);

        return order;
    }
}
