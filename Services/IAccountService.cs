using BankingConsoleAdvanced.Models;

namespace BankingConsoleAdvanced.Services;

public interface IAccountService
{
    Task<Account> GetAccountAsync(string accountNumber, CancellationToken cancellationToken);
    Task DepositAsync(string accountNumber, decimal amount, CancellationToken cancellationToken);
    Task TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount, CancellationToken cancellationToken);
    Task<IReadOnlyList<Account>> GetHighValueAccountsAsync(decimal minimumBalance, CancellationToken cancellationToken);
}
