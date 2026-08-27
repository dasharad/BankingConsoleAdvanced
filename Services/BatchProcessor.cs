using BankingConsoleAdvanced.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BankingConsoleAdvanced.Services;

public sealed class BatchProcessor(
    IOptions<BankingOptions> options,
    ILogger<BatchProcessor> logger)
{
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var batchSize = options.Value.BatchSize;

        for (var i = 1; i <= batchSize; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation(
                "Processing transaction {Number}/{Total}",
                i,
                batchSize);

            await Task.Delay(
                options.Value.TransactionDelayMilliseconds,
                cancellationToken);
        }
    }
}
