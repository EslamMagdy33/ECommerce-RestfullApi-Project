using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Specifications;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Orders.Commands.CreateOrder;
using ECommerce.UseCases.Products.DTOs;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
// Alias on purpose: inside the ECommerce.UseCases.* namespaces, the simple name "Basket"
// resolves to the ECommerce.UseCases.Basket namespace instead of the Basket entity.
using BasketEntity = ECommerce.Domain.Entities.Basket;

namespace ECommerce.UseCases.Tests.Orders;

public class CreateOrderCommandHandlerTests
{
    private readonly Guid userId = Guid.NewGuid();
    private readonly Guid productId = Guid.NewGuid();
    private readonly Guid addressId = Guid.NewGuid();
    private readonly Guid deliveryMethodId = Guid.NewGuid();

    private readonly ICurrentUserService currentUser = Substitute.For<ICurrentUserService>();
    private readonly IBasketStore basketStore = Substitute.For<IBasketStore>();
    private readonly IReadRepository<UserAddress> addressRepository = Substitute.For<IReadRepository<UserAddress>>();
    private readonly IReadRepository<Product> productRepository = Substitute.For<IReadRepository<Product>>();
    private readonly IRepository<DeliveryMethod> deliveryMethodRepository = Substitute.For<IRepository<DeliveryMethod>>();
    private readonly IRepository<Order> orderRepository = Substitute.For<IRepository<Order>>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateOrderCommandHandler handler;
    private Order? savedOrder;

    public CreateOrderCommandHandlerTests()
    {
        currentUser.UserId.Returns(userId);

        var address = UserAddress.Create(
            addressId, userId, "Home", "Ada", "Lovelace", "0100000000",
            "Egypt", "Giza", "1 Pyramids St", "12511").Value!;
        addressRepository
            .FirstOrDefaultAsync(Arg.Any<ISpecification<UserAddress>>(), Arg.Any<CancellationToken>())
            .Returns(address);

        var deliveryMethod = DeliveryMethod.Create(
            deliveryMethodId, "Standard", 20m, "3-5 days").Value!;
        deliveryMethodRepository
            .GetByIdAsync(deliveryMethodId, Arg.Any<CancellationToken>())
            .Returns(deliveryMethod);

        // The basket was cached when the product cost 100, quantity 2
        var basket = BasketEntity.CreateEmpty(userId).Value!;
        basket.AddItem(productId, "Old name", "https://cdn/old.png", 100m, 2);
        basketStore.GetAsync(userId, Arg.Any<CancellationToken>()).Returns(basket);

        orderRepository
            .When(r => r.Add(Arg.Any<Order>()))
            .Do(call => savedOrder = call.Arg<Order>());

        handler = new CreateOrderCommandHandler(
            currentUser,
            basketStore,
            productRepository,
            addressRepository,
            deliveryMethodRepository,
            orderRepository,
            unitOfWork,
            NullLogger<CreateOrderCommandHandler>.Instance);
    }

    private void ProductsInDatabase(params ProductForBasketResponse[] products) =>
        productRepository
            .ListAsync(
                Arg.Any<ISpecification<Product, ProductForBasketResponse>>(),
                Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<ProductForBasketResponse>)products);

    [Fact]
    public async Task Order_uses_current_database_prices_not_the_cached_basket_prices()
    {
        // price changed from 100 to 150 after the item was added to the basket
        ProductsInDatabase(new ProductForBasketResponse(productId, "Fresh name", "https://cdn/fresh.png", 150m));

        var result = await handler.Handle(new CreateOrderCommand(addressId, deliveryMethodId));

        Assert.True(result.IsSuccess);
        Assert.NotNull(savedOrder);
        Assert.Equal(300m, savedOrder!.SubTotal);   // 150 x 2 (quantity still comes from the basket)
        Assert.Equal(320m, savedOrder.Total);       // + 20 delivery
    }

    [Fact]
    public async Task Order_is_rejected_when_a_basket_product_no_longer_exists()
    {
        ProductsInDatabase(); // product was deleted

        var result = await handler.Handle(new CreateOrderCommand(addressId, deliveryMethodId));

        Assert.True(result.IsFailure);
        Assert.Equal(OrderErrors.ProductUnavailable.Code, result.Error!.Code);
        orderRepository.DidNotReceiveWithAnyArgs().Add(default!);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Order_succeeds_even_if_clearing_the_basket_fails()
    {
        ProductsInDatabase(new ProductForBasketResponse(productId, "Fresh name", "https://cdn/fresh.png", 150m));
        basketStore
            .SaveAsync(Arg.Any<BasketEntity>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("redis is down"));

        var result = await handler.Handle(new CreateOrderCommand(addressId, deliveryMethodId));

        Assert.True(result.IsSuccess);
        orderRepository.Received(1).Add(Arg.Any<Order>());
    }
}