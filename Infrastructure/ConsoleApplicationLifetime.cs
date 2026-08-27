namespace BankingConsoleAdvanced.Infrastructure;

public interface IApplicationLifetime
{
    CancellationToken ApplicationStopping { get; }
}

public sealed class ConsoleApplicationLifetime : IApplicationLifetime
{
    private readonly CancellationTokenSource _shutdown = new();

    public CancellationToken ApplicationStopping => _shutdown.Token;

    public void RequestShutdown() => _shutdown.Cancel();
}
