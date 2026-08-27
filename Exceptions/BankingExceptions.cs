namespace BankingConsoleAdvanced.Exceptions;

public class BankingException(string message) : Exception(message);

public sealed class AccountNotFoundException(string accountNumber)
    : BankingException($"Account '{accountNumber}' was not found.");

public sealed class InsufficientBalanceException(
    string accountNumber,
    decimal available,
    decimal requested)
    : BankingException(
        $"Insufficient funds in '{accountNumber}'. Available: ₹{available:N2}; Requested: ₹{requested:N2}.");
