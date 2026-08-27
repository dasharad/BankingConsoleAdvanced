using BankingConsoleAdvanced.Configuration;
using BankingConsoleAdvanced.Events;
using BankingConsoleAdvanced.Exceptions;
using BankingConsoleAdvanced.Models;
using BankingConsoleAdvanced.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BankingConsoleAdvanced.Services;

public sealed class AccountService(
    IRepository<Account> repository,
    TransactionEventPublisher events,
    IOptionsMonitor<BankingOptions> options,
    ILogger<AccountService> logger) : IAccountService
{
    public async Task<Account> GetAccountAsync(
        string accountNumber,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var account = repository.FirstOrDefault(
            a => a.AccountNumber.Equals(
                accountNumber,
                StringComparison.OrdinalIgnoreCase));

        if (account is null)
            throw new AccountNotFoundException(accountNumber);

        await Task.Delay(50, cancellationToken);
        return account;
    }

    public async Task DepositAsync(
        string accountNumber,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var account = await GetAccountAsync(accountNumber, cancellationToken);

        account.Deposit(amount);

        var transaction = new Transaction(
            $"TXN-{Guid.NewGuid():N}"[..12],
            account.AccountNumber,
            TransactionType.Deposit,
            amount,
            DateTimeOffset.UtcNow);

        events.Publish(transaction);

        logger.LogInformation(
            "Deposit completed. Account={AccountNumber}, Amount={Amount}",
            account.AccountNumber,
            amount);

        await Task.Delay(
            options.CurrentValue.TransactionDelayMilliseconds,
            cancellationToken);
    }

    public async Task TransferAsync(
        string fromAccountNumber,
        string toAccountNumber,
        decimal amount,
        CancellationToken cancellationToken)
    {
        if (string.Equals(
            fromAccountNumber,
            toAccountNumber,
            StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Source and destination accounts must be different.");
        }

        var from = await GetAccountAsync(fromAccountNumber, cancellationToken);
        var to = await GetAccountAsync(toAccountNumber, cancellationToken);

        // Business operation: withdraw first; if it fails, destination is untouched.
        from.Withdraw(amount);
        to.Deposit(amount);

        var transaction = new Transaction(
            $"TXN-{Guid.NewGuid():N}"[..12],
            from.AccountNumber,
            TransactionType.Transfer,
            amount,
            DateTimeOffset.UtcNow,
            to.AccountNumber);

        events.Publish(transaction);

        logger.LogInformation(
            "Transfer completed. From={From}, To={To}, Amount={Amount}",
            from.AccountNumber,
            to.AccountNumber,
            amount);

        await Task.Delay(
            options.CurrentValue.TransactionDelayMilliseconds,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Account>> GetHighValueAccountsAsync(
        decimal minimumBalance,
        CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);

        // LINQ + lambda + deferred query + materialization with ToList.
        var result = repository.GetAll()
            .Where(a => a.IsActive)
            .Where(a => a.Balance >= minimumBalance)
            .OrderByDescending(a => a.Balance)
            .ToList();

        return result;
    }
}
