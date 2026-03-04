using EventDrivenCheckout.Basket.Requests;
using FastEndpoints;
using FluentValidation;

namespace EventDrivenCheckout.Basket.Validators;

public class AddItemValidator : Validator<AddItemRequest>
{
    public AddItemValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity has to be greater than 0.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price has to be greater than 0.");
    }
}
