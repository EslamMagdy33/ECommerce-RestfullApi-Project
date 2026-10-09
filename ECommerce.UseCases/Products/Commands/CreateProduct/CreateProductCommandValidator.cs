using ECommerce.Domain.Entities;
using FluentValidation;

namespace ECommerce.UseCases.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode("Product.InvalidName")
            .WithMessage("Product name is required.")
            .MaximumLength(Product.MaxNameLength)
            .WithErrorCode("Product.NameTooLong")
            .WithMessage($"Product name cannot exceed {Product.MaxNameLength} characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithErrorCode("Product.InvalidDescription")
            .WithMessage("Product description is required.")
            .MaximumLength(Product.MaxDescriptionLength)
            .WithErrorCode("Product.DescriptionTooLong")
            .WithMessage($"Product description cannot exceed {Product.MaxDescriptionLength} characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithErrorCode("Product.InvalidPrice")
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.Image)
            .NotNull()
            .WithErrorCode("Product.InvalidPictureUrl")
            .WithMessage("Product picture is required.")
            .Must(image => image is { Length: > 0 })
            .WithErrorCode("Product.InvalidPictureUrl")
            .WithMessage("Product picture is required.");

        RuleFor(x => x.ProductBrandId)
            .NotEmpty()
            .WithErrorCode("Product.InvalidBrand")
            .WithMessage("Product brand is required.");

        RuleFor(x => x.ProductTypeId)
            .NotEmpty()
            .WithErrorCode("Product.InvalidType")
            .WithMessage("Product type is required.");
    }
}
