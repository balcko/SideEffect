// See https://aka.ms/new-console-template for more information

using System.Transactions;
using Abstractions;

namespace Sample;

internal static class Program
{
    static async Task Main()
    {
        // TODO resolve dependencies from DI
        IDepositRepository _depositRepository = null!;
        ISideEffectPlanner _sideEffectPlanner = null!;

        const long depositAmount = 10L;

        using (var scope = new TransactionScope())
        {
            var depositId = await _depositRepository.Insert(depositAmount, CancellationToken.None);
            await _sideEffectPlanner.Plan(new SendPlatformEventSideEffect<DepositCreated>(new DepositCreated(depositId, depositAmount)), CancellationToken.None);
            scope.Complete();
        }
    }
}