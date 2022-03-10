namespace Abstractions.DataAccess;

/// <summary>
/// repository responsible for persisting planned side effects until execution time
/// </summary>
public interface ISideEffectRepository
{
    /// <summary>
    /// saves planned side effect as part of same DB transaction with other changes within business process
    /// </summary>
    /// <param name="payload">serialized side affect</param>
    /// <param name="plannedAt">should be current datetime</param>
    /// <param name="executeAt">time in the future when the side effect should be executed soonest after failure</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ISideEffectRecord> Save(byte[] payload, DateTimeOffset plannedAt, DateTimeOffset executeAt, CancellationToken cancellationToken);

    /// <summary>
    /// deletes side effect records, which we already successfully processed
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Delete(IEnumerable<long> ids, CancellationToken cancellationToken);

    /// <summary>
    /// reads side effect records which should be already executed and shifts executeAt on returned records
    /// records returned should be ordered by executeAt in ascending order
    /// </summary>
    /// <param name="minExecuteAt">records where executeAt property is greater or equal to this value are returned only</param>
    /// <param name="executeAtShift">executeAt property of returned records will be shifted by this time span to the future</param>
    /// <param name="maxCount">maximum count of records to be returned</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<ISideEffectRecord>> GetForExecution(DateTimeOffset minExecuteAt, TimeSpan executeAtShift, long maxCount, CancellationToken cancellationToken);
}