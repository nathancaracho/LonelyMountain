using FluentValidation;

namespace LonelyMountain.Example
{
    public class CustomerValidator : AbstractValidator<CustomerMessage>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer.Name)
            .NotEmpty()
            .WithMessage("The [Name] can't be null or empty");

            RuleFor(customer => customer.Age)
            .NotEmpty()
            .WithMessage("The [Age] can't be null")
            .LessThan(18)
            .WithMessage("The customer can't not be younger than 18 years.");

        }


    }
}