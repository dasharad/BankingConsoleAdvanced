using BankingConsoleAdvanced.Configuration;
using BankingConsoleAdvanced.Events;
using BankingConsoleAdvanced.Extensions;
using BankingConsoleAdvanced.Infrastructure;
using BankingConsoleAdvanced.Models;
using BankingConsoleAdvanced.Repositories;
using BankingConsoleAdvanced.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ================================================================
// ADVANCED BANKING CONSOLE APPLICATION

//
// Covered:
// Modern syntax, OOP, composition, SOLID, generics, collections,
// LINQ, lambda, delegates/events, extension methods, records,
// pattern matching, nullable reference types, exception handling,
// async/await, Tasks, CancellationToken, concurrency/parallelism,
// memory/performance basics, SDK/NuGet, configuration, DI,
// Options pattern, logging, environments, application lifecycle,
// and modern project structure.
// ================================================================

Console.WriteLine("============================================================");
Console.WriteLine(" ADVANCED BANKING CONSOLE APPLICATION - C# 14 / .NET 10");
Console.WriteLine("============================================================");
Console.WriteLine();

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? "Production";

Console.WriteLine($"Environment : {environment}");
Console.WriteLine();

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddSimpleConsole(options =>
    {
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

services.Configure<BankingOptions>(options =>
{
    options.BankName = "Demo National Bank";
    options.TransactionDelayMilliseconds = 150;
    options.BatchSize = 8;
});

services.AddSingleton<IApplicationLifetime, ConsoleApplicationLifetime>();
services.AddSingleton<TransactionEventPublisher>();
services.AddSingleton<IRepository<Account>, InMemoryRepository<Account>>();
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<BankReportService>();
services.AddScoped<BatchProcessor>();

using var provider = services.BuildServiceProvider();

var lifetime = provider.GetRequiredService<IApplicationLifetime>();
var eventPublisher = provider.GetRequiredService<TransactionEventPublisher>();
var repository = provider.GetRequiredService<IRepository<Account>>();
var accountService = provider.GetRequiredService<IAccountService>();
var reportService = provider.GetRequiredService<BankReportService>();
var batchProcessor = provider.GetRequiredService<BatchProcessor>();

// ---------------------------------------------------------------
// Events + delegates
// ---------------------------------------------------------------
eventPublisher.TransactionCompleted += (_, transaction) =>
{
    Console.WriteLine(
        $"EVENT -> {transaction.Type} | {transaction.Id} | " +
        $"{transaction.AccountNumber} | ₹{transaction.Amount:N2}");
};

// ---------------------------------------------------------------
// Generics + Collections
// ---------------------------------------------------------------
var ravi = new Customer(1, "Ravi", "ravi@bank.test");
var suresh = new Customer(2, "Suresh", "suresh@bank.test");
var anil = new Customer(3, "Anil", "anil@bank.test");

repository.Add(
    new SavingsAccount("ACC1001", ravi, 150_000m, 6.5m));

repository.Add(
    new CurrentAccount("ACC1002", suresh, 50_000m, 100_000m));

repository.Add(
    new SavingsAccount("ACC1003", anil, 1_250_000m, 7.0m));
Console.WriteLine("=== COLLECTIONS + GENERICS ===");

foreach (var account in repository.GetAll())
{
    Console.WriteLine(account);
}

// Dictionary: O(1)-average key lookup.
var accountDictionary = repository.GetAll()
    .ToDictionary(a => a.AccountNumber);

Console.WriteLine(
    $"Dictionary lookup ACC1001 -> {accountDictionary["ACC1001"].DisplayName()}");

// HashSet: uniqueness.
var uniqueTransactionIds = new HashSet<string>
{
    "TXN001",
    "TXN002",
    "TXN001"
};



Console.WriteLine(
    $"HashSet unique transaction IDs -> {uniqueTransactionIds.Count}");

Console.WriteLine();

// ---------------------------------------------------------------
// LINQ + Lambda + Extension Methods
// ---------------------------------------------------------------
reportService.PrintSummary();

var highValue = await accountService
    .GetHighValueAccountsAsync(100_000m, lifetime.ApplicationStopping);

Console.WriteLine();
Console.WriteLine("=== HIGH VALUE ACCOUNTS ===");

foreach (var account in highValue)
{
    Console.WriteLine(
        $"{account.DisplayName()} | Premium={account.IsPremium()}");
}

Console.WriteLine();

// ---------------------------------------------------------------
// Pattern matching
// ---------------------------------------------------------------
Console.WriteLine("=== PATTERN MATCHING ===");

foreach (var account in repository.GetAll())
{
    var category = account switch
    {
        SavingsAccount { Balance: >= 1_000_000m } => "Premium Savings",
        SavingsAccount => "Savings",
        CurrentAccount { Balance: >= 100_000m } => "High-value Current",
        CurrentAccount => "Current",
        _ => "Unknown"
    };

    Console.WriteLine($"{account.AccountNumber} -> {category}");
}

Console.WriteLine();

// ---------------------------------------------------------------
// async / await + Task
// ---------------------------------------------------------------
Console.WriteLine("=== ASYNC / AWAIT + TASK.WHENALL ===");

var balanceTasks = repository.GetAll()
    .Select(account =>
        accountService.GetAccountAsync(
            account.AccountNumber,
            lifetime.ApplicationStopping))
    .ToArray();

var balances = await Task.WhenAll(balanceTasks);

foreach (var account in balances)
{
    Console.WriteLine(
        $"{account.AccountNumber} -> ₹{account.Balance:N2}");
}

Console.WriteLine();

// ---------------------------------------------------------------
// Transfer + custom exceptions
// ---------------------------------------------------------------
Console.WriteLine("=== EXCEPTION HANDLING ===");

try
{
    await accountService.TransferAsync(
        "ACC1001",
        "ACC1002",
        25_000m,
        lifetime.ApplicationStopping);

    Console.WriteLine("Transfer succeeded.");
}
catch (InsufficientBalanceException ex)
{
    Console.WriteLine($"Business exception: {ex.Message}");
}
catch (AccountNotFoundException ex)
{
    Console.WriteLine($"Not found: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected exception: {ex.Message}");
}

try
{
    await accountService.TransferAsync(
        "ACC1001",
        "UNKNOWN",
        5_000m,
        lifetime.ApplicationStopping);
}
catch (BankingException ex)
{
    Console.WriteLine($"Expected banking error: {ex.Message}");
}

Console.WriteLine();

// ---------------------------------------------------------------
// CancellationToken + linked cancellation
// ---------------------------------------------------------------
Console.WriteLine("=== CANCELLATION TOKEN ===");

using var timeoutCts =
    new CancellationTokenSource(
        TimeSpan.FromSeconds(5));

using var userCts =
    new CancellationTokenSource();

using var linkedCts =
    CancellationTokenSource.CreateLinkedTokenSource(
        timeoutCts.Token,
        userCts.Token,
        lifetime.ApplicationStopping);

var batchTask = batchProcessor.ProcessAsync(
    linkedCts.Token);

await Task.Delay(600);

Console.WriteLine("Requesting cooperative cancellation...");
userCts.Cancel();

try
{
    await batchTask;
}
catch (OperationCanceledException)
{
    Console.WriteLine("Batch processing cancelled safely.");
}

Console.WriteLine();

// ---------------------------------------------------------------
// Concurrency / Parallelism
// ---------------------------------------------------------------
Console.WriteLine("=== CONCURRENCY / PARALLELISM ===");

var parallelAccounts = repository.GetAll().ToArray();

Parallel.ForEach(
    parallelAccounts,
    account =>
    {
        // CPU-bound demonstration only.
        var classification =
            account.Balance >= 1_000_000m
                ? "Premium"
                : "Standard";

        Console.WriteLine(
            $"Parallel worker -> {account.AccountNumber}: {classification}");
    });

Console.WriteLine();

// ---------------------------------------------------------------
// Records + with expression
// ---------------------------------------------------------------
Console.WriteLine("=== RECORDS + WITH EXPRESSION ===");

var customer = new Customer(
    99,
    "Original Customer",
    "old@bank.test");

var updatedCustomer = customer with
{
    Email = "new@bank.test"
};

Console.WriteLine($"Original : {customer}");
Console.WriteLine($"Updated  : {updatedCustomer}");

Console.WriteLine();

// ---------------------------------------------------------------
// Application lifecycle
// ---------------------------------------------------------------
Console.WriteLine("=== APPLICATION LIFECYCLE ===");
Console.WriteLine("Services registered -> application started.");
Console.WriteLine("Cancellation source registered for graceful shutdown.");

// Demonstrate application stopping token without cancelling
// the real process during normal execution.
Console.WriteLine(
    $"Application stopping requested: {lifetime.ApplicationStopping.IsCancellationRequested}");

Console.WriteLine();

// ---------------------------------------------------------------
// Final balances
// ---------------------------------------------------------------
Console.WriteLine("=== FINAL ACCOUNT STATE ===");

foreach (var account in repository.GetAll())
{
    Console.WriteLine(account);
}

Console.WriteLine();
Console.WriteLine("============================================================");
Console.WriteLine(" DEMO COMPLETED SUCCESSFULLY");
Console.WriteLine("============================================================");
