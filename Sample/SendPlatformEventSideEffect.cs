using Abstractions;

namespace Sample;

public sealed class SendPlatformEventSideEffect<T> : ISideEffect where T : class
{
    public SendPlatformEventSideEffect(T platformEvent)
    {
        PlatformEvent = platformEvent;
    }
    public T PlatformEvent { get; }
}