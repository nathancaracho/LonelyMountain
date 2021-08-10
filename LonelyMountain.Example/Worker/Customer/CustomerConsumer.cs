using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentValidation;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queues;

namespace LonelyMountain.Example.Worker.Consumer
{
    [ActiveQueue("insert-customer")]
    public class CustomerConsumer : AbstractConsumer<CustomerMessage>
    {
        public CustomerConsumer(IValidator<CustomerMessage> validator) : base(validator) { }
        protected override Task<Result> Action(CustomerMessage message) =>
            Task.FromResult(Result.Success());
    }
}