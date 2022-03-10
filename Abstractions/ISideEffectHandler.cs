namespace Abstractions;

/// <summary>
/// handler responsible for execution of specific side effect (all side effect types should have its handler)
/// </summary>
public interface ISideEffectHandler
{
    /// <summary>
    /// pre-check if this handler might process side effect
    /// </summary>
    /// <param name="sideEffect"></param>
    /// <returns></returns>
    bool Supports(ISideEffect sideEffect);

    /// <summary>
    /// execution of side effect itself
    /// should be retried later in case it throws
    /// </summary>
    /// <param name="sideEffect"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Handle(ISideEffect sideEffect, CancellationToken cancellationToken);
}