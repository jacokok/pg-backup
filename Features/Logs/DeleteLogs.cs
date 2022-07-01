using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
namespace PGBackup.Features.Logs;

public class DeleteLogs : EndpointWithoutRequest<List<Log>>
{
    private readonly DBConfig _config;

    public DeleteLogs(IOptions<DBConfig> config)
    {
        _config = config.Value;
    }

    public override void Configure()
    {
        Delete("/logs");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var sql = $"DELETE FROM {_config.LogTable}";
        using var connection = new NpgsqlConnection(_config.LogsConnectionString);
        connection.Open();
        var result = await connection.ExecuteAsync(sql);
        if (result > 0)
        {
            await SendNoContentAsync(cancellationToken);
        }
        else
        {
            await SendNotFoundAsync(cancellationToken);
        }

    }
}