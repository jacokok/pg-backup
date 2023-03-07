using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
namespace PGBackup.Features.Logs;

public class GetLogs : EndpointWithoutRequest<List<Log>>
{
    private readonly DBConfig _config;

    public GetLogs(IOptions<DBConfig> config)
    {
        _config = config.Value;
    }

    public override void Configure()
    {
        Get("/logs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var sql = $"SELECT * FROM Logs ORDER BY timestamp DESC";
        using var connection = new SqliteConnection(_config.LogsConnectionString);

        connection.Open();
        var result = await connection.QueryAsync<Log>(sql);
        await SendAsync(result.ToList(), cancellation: cancellationToken);
    }
}