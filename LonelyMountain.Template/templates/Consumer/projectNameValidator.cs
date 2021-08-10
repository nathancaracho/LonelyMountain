using FluentValidation;

namespace projectName
{
    public class projectNameValidator : AbstractValidator<projectNameMessage>
    {
        public projectNameValidator()
        {
            RuleFor(customer => customer.Name)
            .NotEmpty()
            .WithMessage("The [Name] can't be null or empty");
        }


    }
}