# Advanced Banking Console Application

A practical C# 14 / .NET 10 console application designed for an experienced .NET developer.

## Concepts covered

### C#
- Modern C# syntax
- Classes, interfaces, inheritance
- Composition
- Encapsulation
- Abstraction
- SOLID principles
- Generics
- Collections
- LINQ
- Lambda expressions
- 
- gates
- Events
- Extension methods
- Records
- Pattern matching
- Nullable reference types
- Exception handling
- async/await
- Tasks
- CancellationToken
- CancellationTokenSource.CreateLinkedTokenSource
- Concurrency and parallelism
- Memory/performance fundamentals

### Modern .NET
- .NET 10 / C# 14
- SDK and CLI
- Project structure
- NuGet package references
- Dependency Injection
- Options pattern
- Logging
- Environment detection
- Application lifecycle
- Modern project organization

## Architecture

```text
BankingConsoleAdvanced
|
+-- Configuration
|   +-- BankingOptions.cs
|
+-- Events
|   +-- BankEvents.cs
|
+-- Exceptions
|   +-- BankingExceptions.cs
|
+-- Extensions
|   +-- AccountExtensions.cs
|
+-- Infrastructure
|   +-- ConsoleApplicationLifetime.cs
|
+-- Models
|   +-- Account.cs
|   +-- Customer.cs
|   +-- Transaction.cs
|
+-- Repositories
|   +-- IRepository.cs
|   +-- InMemoryRepository.cs
|
+-- Services
|   +-- IAccountService.cs
|   +-- AccountService.cs
|   +-- BankReportService.cs
|   +-- BatchProcessor.cs
|
+-- Program.cs
+-- BankingConsoleAdvanced.csproj
```

## Prerequisites

Install the .NET 10 SDK.

Verify:

```bash
dotnet --version
```

The project explicitly targets:

```xml
<TargetFramework>net10.0</TargetFramework>
<LangVersion>14.0</LangVersion>
```

## Run

From the project directory:

```bash
dotnet restore
dotnet build
dotnet run
```

The application uses only NuGet packages from Microsoft for dependency injection, logging, and options.

## Expected output

The exact timestamps and transaction IDs will differ between runs.

The output should contain sections similar to:

```text
============================================================
 ADVANCED BANKING CONSOLE APPLICATION - C# 14 / .NET 10
============================================================

Environment : Production

=== COLLECTIONS + GENERICS ===
ACC1001  | Ravi       | SavingsAccount   | ₹  150,000.00 | Active=True
ACC1002  | Suresh     | CurrentAccount   | ₹   50,000.00 | Active=True
ACC1003  | Anil       | SavingsAccount   | ₹1,250,000.00 | Active=True
Dictionary lookup ACC1001 -> Ravi (ACC1001)
HashSet unique transaction IDs -> 2

=== LINQ REPORT ===
SavingsAccount    Count=2  Total=₹1,400,000.00 Average=₹700,000.00
CurrentAccount    Count=1  Total=₹50,000.00 Average=₹50,000.00
Any premium/high-value account : True
All accounts premium            : False

=== HIGH VALUE ACCOUNTS ===
Anil (ACC1003) | Premium=True
Ravi (ACC1001) | Premium=False

=== PATTERN MATCHING ===
ACC1001 -> Savings
ACC1002 -> Current
ACC1003 -> Premium Savings

=== ASYNC / AWAIT + TASK.WHENALL ===
ACC1001 -> ₹150,000.00
ACC1002 -> ₹50,000.00
ACC1003 -> ₹1,250,000.00

=== EXCEPTION HANDLING ===
EVENT -> Transfer | TXN-... | ACC1001 | ₹25,000.00
Transfer succeeded.
Expected banking error: Account 'UNKNOWN' was not found.

=== CANCELLATION TOKEN ===
Requesting cooperative cancellation...
Batch processing cancelled safely.

=== CONCURRENCY / PARALLELISM ===
Parallel worker -> ...

=== RECORDS + WITH EXPRESSION ===
Original : Customer { Id = 99, Name = Original Customer, Email = old@bank.test }
Updated  : Customer { Id = 99, Name = Original Customer, Email = new@bank.test }

=== APPLICATION LIFECYCLE ===
Services registered -> application started.
Cancellation source registered for graceful shutdown.
Application stopping requested: False

=== FINAL ACCOUNT STATE ===
...
DEMO COMPLETED SUCCESSFULLY
```

## Important learning notes

### 1. async/await

The sample intentionally uses `Task.Delay` to simulate I/O such as a database or external API. It does not create a new thread merely because `async` is used.

### 2. Task.WhenAll

The balance operations are independent, so they are started together and awaited with `Task.WhenAll`.

### 3. CancellationToken

The batch processor supports cooperative cancellation and checks the token before every unit of work.

### 4. Linked cancellation

The sample combines:
- user cancellation
- timeout cancellation
- application shutdown cancellation

using `CancellationTokenSource.CreateLinkedTokenSource`.

### 5. Parallel.ForEach

The example is deliberately CPU-bound. Do not automatically use `Parallel.ForEach` for database/API calls. For I/O-heavy workloads, asynchronous APIs are normally the better approach.

### 6. SOLID

The sample separates:
- repository responsibility
- account/business responsibility
- reporting responsibility
- batch-processing responsibility
- configuration
- event publishing

### 7. Encapsulation

`Account.Balance` has a private setter. Consumers must use `Deposit` or `Withdraw`.

## Suggested exercises

After running the project, try these:

1. Add a `LoanAccount`.
2. Add transaction history using `ConcurrentDictionary`.
3. Add `HashSet` based duplicate transaction detection.
4. Add a `Queue<Transaction>` for pending transactions.
5. Add retry logic using a cancellation token.
6. Add a `FraudChecker` interface and two implementations.
7. Add `IOptionsMonitor` configuration changes.
8. Add a custom validation extension method.
9. Add a `SelectMany` report for customers with multiple accounts.
10. Add unit tests for `AccountService`.
11. Replace the in-memory repository with EF Core.
12. Convert the console application into ASP.NET Core Web API.
