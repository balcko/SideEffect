namespace Sample;

public class DepositCreated
{
    public DepositCreated(long depositId, long amountCents)
    {
        DepositId = depositId;
        AmountCents = amountCents;
    }
    public long DepositId { get; }
    public long AmountCents { get; }
}