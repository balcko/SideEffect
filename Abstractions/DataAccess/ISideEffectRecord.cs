namespace Abstractions.DataAccess;

/// <summary>
/// persisted side effect record
/// </summary>
public interface ISideEffectRecord
{
    /// <summary>
    /// database id of side effect
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// serialized side effect payload
    /// </summary>
    public byte[] Payload { get; }

    /// <summary>
    /// time when the side effect was planned (inserted into DB), transaction was still open at that time
    /// </summary>
    public DateTimeOffset PlannedAt { get; }

    /// <summary>
    /// soonest time when the persisted side effect should be picked up and tried to execute after initial failure or skipped execution
    /// </summary>
    public DateTimeOffset ExecuteAt { get; }
}