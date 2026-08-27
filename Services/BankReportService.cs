using BankingConsoleAdvanced.Models;
using BankingConsoleAdvanced.Repositories;

namespace BankingConsoleAdvanced.Services;

public sealed class BankReportService(
    IRepository<Account> accountRepository)
{
    public void PrintSummary()
    {
        var accounts = accountRepository.GetAll();

        Console.WriteLine();
        Console.WriteLine("=== LINQ REPORT ===");

        var grouped = accounts
            .GroupBy(a => a.GetType().Name)
            .Select(g => new
            {
                Type = g.Key,
                Count = g.Count(),
                TotalBalance = g.Sum(a => a.Balance),
                AverageBalance = g.Average(a => a.Balance)
            })
            .OrderByDescending(x => x.TotalBalance);

        foreach (var item in grouped)
        {
            Console.WriteLine(
                $"{item.Type,-18} Count={item.Count,-2} " +
                $"Total=₹{item.TotalBalance:N2} " +
                $"Average=₹{item.AverageBalance:N2}");
        }

        var allPremium = accounts.All(a => a.IsPremium());
        var anyHighValue = accounts.Any(a => a.Balance >= 1_000_000m);

        Console.WriteLine($"Any premium/high-value account : {anyHighValue}");
        Console.WriteLine($"All accounts premium            : {allPremium}");
    }
}
