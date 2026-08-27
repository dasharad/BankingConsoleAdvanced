namespace BankingConsoleAdvanced.Models;

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer
}

public sealed record Transaction(
    string Id,
    string AccountNumber,
    TransactionType Type,
    decimal Amount,
    DateTimeOffset CreatedAt,
    string? RelatedAccountNumber = null);
