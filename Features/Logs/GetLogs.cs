using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
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
        var sql = $"SELECT * FROM {_config.LogTable} ORDER BY timestamp DESC fetch first 100 rows only";
        using var connection = new NpgsqlConnection(_config.LogsConnectionString);

        connection.Open();
        var result = await connection.QueryAsync<Log>(sql);
        await SendAsync(result.ToList(), cancellation: cancellationToken);
    }
}