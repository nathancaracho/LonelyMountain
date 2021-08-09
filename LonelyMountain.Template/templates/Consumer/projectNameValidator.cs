using FluentValidation;

namespace projectName
{
    public class ExampleValidator : AbstractValidator<projectNameMessage>
    {
        public ExampleValidator()
        {
            RuleFor(projectName => projectName.Name)
            .NotEmpty()
            .WithMessage("The [Name] can't be null or empty");

        }


    }
}