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
        protected override async Task<Result> Action(CustomerMessage message, IAcknowledgeManager acknowledge)
        {
            await acknowledge.BasicAck();
            return await Task.FromResult(Result.Success());
        }
    }
}