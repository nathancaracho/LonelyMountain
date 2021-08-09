using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;

namespace LonelyMountain.Src.Consumer
{
    public abstract class AbstractConsumer<TMessage> : IConsumer, IConsumer<TMessage>
    {
        private readonly IValidator<TMessage> _validator;

        public AbstractConsumer(IValidator<TMessage> validator) => _validator = validator;

        protected abstract Task<Result> Action(TMessage message);

        protected Result<TMessage> ParseMessage(byte[] rawMessage)
        {
            try
            {
                return Result.Success(JsonSerializer.Deserialize<TMessage>(Encoding.UTF8.GetString(rawMessage)));
            }
            catch (Exception ex)
            {
                return Result.Failure<TMessage>($"Error when try deserialize message body {ex.Message}");
            }
        }

        protected async Task<Result<TMessage>> Validate(TMessage message)
        {
            var messageValidate = await _validator.ValidateAsync(message);

            if (messageValidate.IsValid is false)
                return Result.Failure<TMessage>(GetErrorFromResult(messageValidate));

            return Result.Success(message);
        }
        private string GetErrorFromResult(ValidationResult validation) =>
            string.Join("; \n",validation.Errors.Select(error => String.Join(", ", error.ErrorMessage)) );


        public async Task<Result> ProcessMessage(byte[] rawMessage) =>
           await ParseMessage(rawMessage)
           .Check(message => Validate(message))
           .Check(message => Action(message));

    }
}