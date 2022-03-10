using Abstractions.DataAccess;

namespace Abstractions;

/// <summary>
/// component responsible for actual processing (execution) of side effects
/// it should also periodically get side effects for execution from DB in the background in order to eventually process side effects, where initial execution failed/was skipped e.g. due to process crash
/// </summary>
public interface ISideEffectProcessor
{
    /// <summary>
    /// tells processor to schedule execution of these side effects immediately (as soon as possible), ideally in asynchronous manner
    /// called for each side effect planned during DB transaction after successful commit
    /// </summary>
    /// <param name="sideEffect"></param>
    /// <param name="sideEffectRecord"></param>
    void ScheduleImmediately(ISideEffect sideEffect, ISideEffectRecord sideEffectRecord);
}