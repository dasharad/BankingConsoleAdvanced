using BankingConsoleAdvanced.Models;

namespace BankingConsoleAdvanced.Events;

public delegate void TransactionCompletedHandler(
    object? sender,
    Transaction transaction);

public sealed class TransactionEventPublisher
{
    public event TransactionCompletedHandler? TransactionCompleted;

    public void Publish(Transaction transaction) =>
        TransactionCompleted?.Invoke(this, transaction);
}
