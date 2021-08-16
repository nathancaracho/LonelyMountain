<p align="center">
  <img height="140" src="https://github.com/nathancaracho/LonelyMountain/blob/main/Docs/logo.png?raw=true">
</p>

# Lonely Mountain 　　　　　　　　　　　　　　　　[\[todo\]](https://tranquil-bench-d6f.notion.site/7f4a2771d0834531a78c4fabd13cca53?v=6592141c21784927a5737fa16ffed032)

[Lonely Mountain](http://tolkiengateway.net/wiki/Lonely_Mountain) is a `consumer` wrapper library and `consumer` template, the consumer project is composing by one `Worker` to one or many `Consumers`. The `Worker` is root folder project and wrapper all `Consumers` with one `appsettings`, this `appsettings` is shared with others `Consumers`. The `Consumer` is a queue process project composing of three components [Message](about:blank#Message), [Validator](about:blank#Validator), and [Consumer](about:blank#Consumer).


## Project structure
``` text
ROOT \
┣ LonelyMountain.Src \
┃ ┣ 📂 Connection \
┃ ┣ 📂 Consumer \
┃ ┣ 📂 Ioc \
┃ ┣ 📂 Queue \
┃ ┣ 📂 Subscriber \
┃ ┣ ┣  Bootstrap.cs 
┃ ┣ ┣  Worker.cs 
┃ ┗ ┗  LonelyMountain.Src.csproj
┃
┣ LonelyMountain.Template \
┃ ┣ 📂 templates \
┃ ┣ 📂 Consumer \
┃ ┣ ┣ 📂 .template.config \
┃ ┣ ┣ ┣  ┗ LonelyMountain.Src.csproj
┃ ┣ ┣ ┣  projectNameConsumer.cs 
┃ ┣ ┣ ┣  projectNameMessage.cs 
┃ ┣ ┣ ┣  projectNameValidator.cs 
┃ ┣ ┣ ┗  program.cs
┃ ┗  LonelyMountain.Template.csproj
┃
┣ LonelyMountain.Example \
┃ ┣ 📂 Worker \
┃ ┣ ┣ 📂 Customer \
┃ ┣ ┣ ┣  LonelyMountain.Example.csproj
┃ ┣ ┣ ┣  CustomerConsumer.cs 
┃ ┣ ┣ ┣  CustomerMessage.cs 
┃ ┣ ┣ ┣  CustomerValidator.cs 
┃ ┗ ┗ ┗  program.cs
┃
┣  docker-compose.yml
┣  README.md
┗  LonelyMountain.sln
```
## Consumer Steps

When a consumer is triggered the following steps start

[Message parse](about:blank#The-message-parse), [Message validation](about:blank#The-message-validation), [Message Processing](about:blank#The-message-processing), and [Message queue management](about:blank#Message-queue-management)

, if any step failure the other steps not is called.

<p align="center">
  <img height="400" src="https://github.com/nathancaracho/LonelyMountain/blob/main/Docs/cunsumer-steps.png?raw=true](https://github.com/nathancaracho/LonelyMountain/blob/main/Docs/cunsumer-steps.png?raw=true">
</p>


### The message parses is the step when a queued message is parsed to an entity, the message only accepts JSON formats and must have the same structure as entity.

### The message validation

The message validation step will validate the parsed entity using [Fluent Validation](https://fluentvalidation.net/). This step ensures the structure is exactly what the consumer needs.

### The message processing

The message processing is the step when the message is processed, persisted, or else.

### Message queue management

The message queue management is when the message is discarded when successful or sent to `dead letter` if any previous step fails.

## Consumer components

The worker consumer is composed of [Message](about:blank#Message), [Validator](about:blank#Validator), and [Consumer](about:blank#Consumer).

### Message

The message is the queued entity.
```csharp
    public record CustomerMessage(string Name, string Identity, int Age);
```
### Validator
The validator is the message validator using [Fluent Validation](https://fluentvalidation.net/)
```csharp
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
```

### Consumer
The consumer is the component when the message is processed using a [Monad](https://en.wikipedia.org/wiki/Monad_(functional_programming)) to enchain other components.
The `Queue` annotation is used to categorize in Active queue (`ActiveQueue`), Dead letter queue (`DlqQueue`) and Topic (`Topic`).

```csharp
    [ActiveQueue("insert-customer")]
    public class CustomerConsumer : AbstractConsumer<CustomerMessage>
    {
        public CustomerConsumer(IValidator<CustomerMessage> validator) : base(validator) { }
        protected override async Task<Result> Action(CustomerMessage message,IAcknowledgeManager acknowledge){
            await acknowledge.BasicAck();
            return await  Task.FromResult(Result.Success());
        }
    }
```

The consumer use [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) to wrap the result on a monad. The consumer result can be `Success` or `Failure`, the `Failure` must contain the failure reason.
Failure
```csharp
    protected override Task<Result> Action(Message message,IAcknowledgeManager acknowledge) =>
        return Task.FromResult(Result.Failure("Some error happened when I try process the message"));
```

## How to use template
> The template is not on nuget yet.  


Install lm-consumer  template
```bash
dotnet new --install LonelyMountain.Consumer
```
After install template create template using sourcing name
```bash
dotnet new lm-consumer -o YourProject.Worker.Customer
```
Generated folder
```text
YourProject \
┣ 📂 Worker \
┣ ┣ 📂 Customer \
┣ ┣ ┣  YourProject.Worker.Customer.csproj
┣ ┣ ┣  CustomerConsumer.cs 
┣ ┣ ┣  CustomerMessage.cs 
┣ ┣ ┣  CustomerValidator.cs 
┣ ┣ ┗  program.cs
┣ ┣ YourProject.Worker.csproj
┗ ┗ appsettings.json
```
## How to run
```bash
dotnet run -p YourProject.Worker.Customer
info: YourProject.Src.Worker[0]
      Worker start running at: 08/09/2021 01:09:53 -03:00
info: YourProject.Src.Subscriber.RabbitMQSubscriber[0]
      Start subscribing YourProject.Src.Queue.ActiveQueue consumer
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production

info: YourProject.Src.Subscriber.RabbitMQSubscriber[0]
      The insert-customer consumer was triggered
fail: YourProject.Src.Subscriber.RabbitMQSubscriber[0]
      An error was occurred when try processing insert-customer. Error: Error when try deserialize message body 'test' is an invalid JSON literal. Expected the literal 'true'. Path: $ | LineNumber: 0 | BytePositionInLine: 1.
info: YourProject.Src.Subscriber.RabbitMQSubscriber[0]
      The insert-customer was processed with success

```