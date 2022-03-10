namespace Abstractions;

/// <summary>
/// class used to reliably plan execution of side effect bound to current DB transaction
/// will execute only if DB transaction is committed (at least once semantics, consumer of side effect should be idempotent)
/// </summary>
public interface ISideEffectPlanner
{
    Task Plan(ISideEffect sideEffect, CancellationToken cancellationToken);
}