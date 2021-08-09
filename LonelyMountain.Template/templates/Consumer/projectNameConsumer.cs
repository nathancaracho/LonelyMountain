using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentValidation;
using LonelyMountain.Src.Consumer;
using LonelyMountain.Src.Queue;

namespace projectName
{
    [ActiveQueue("insert-projectName")]
    public class projectNameConsumer : AbstractConsumer<projectNameMessage>
    {
        public projectNameConsumer(IValidator<projectNameMessage> validator) : base(validator) { }
        protected override Task<Result> Action(projectNameMessage message) =>
            Task.FromResult(Result.Success());
    }
}