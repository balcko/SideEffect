namespace Sample;

public interface IDepositRepository
{
    Task<long> Insert(long amountCents, CancellationToken cancellationToken);
}