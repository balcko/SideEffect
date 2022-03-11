using Abstractions;

namespace Sample;

public class PlatformEventHandler : ISideEffectHandler
{
    public bool Supports(ISideEffect sideEffect) => sideEffect is SendPlatformEventSideEffect<DepositCreated>;

    public Task Handle(ISideEffect sideEffect, CancellationToken cancellationToken)
    {
        // sends the platform event to ActiveMq
        return Task.CompletedTask;
    }
}