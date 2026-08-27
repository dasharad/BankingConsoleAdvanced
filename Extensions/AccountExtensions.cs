using BankingConsoleAdvanced.Models;

namespace BankingConsoleAdvanced.Extensions;

public static class AccountExtensions
{
    public static bool IsPremium(this Account account) =>
        account.Balance >= 1_000_000m;

    public static string DisplayName(this Account account) =>
        $"{account.Customer.Name} ({account.AccountNumber})";
}
