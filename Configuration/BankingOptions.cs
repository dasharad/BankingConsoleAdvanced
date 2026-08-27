namespace BankingConsoleAdvanced.Configuration;

public sealed class BankingOptions
{
    public string BankName { get; set; } = "Demo National Bank";
    public int TransactionDelayMilliseconds { get; set; } = 250;
    public int BatchSize { get; set; } = 10;
}
