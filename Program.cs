global using FastEndpoints;
using FastEndpoints.Swagger;
using PGBackup;
using PGBackup.Extensions;
using PGBackup.Jobs;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

DBConfig dBConfig = new();
builder.Configuration.GetSection("DBConfig").Bind(dBConfig);

builder.Host.UseSerilog();

var path = Path.GetFullPath("logs.sqlite");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.SQLite(path, "Logs")
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information("Starting up");

try
{
    Directory.CreateDirectory("backup");

    builder.Services.AddFastEndpoints();
    builder.Services.AddCors(options =>
    {
        string cors = builder.Configuration.GetValue<string>("Cors") ?? "";
        options.AddPolicy(name: "AllowedOrigins",
        builder =>
        {
            builder.WithOrigins(cors)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
    builder.Services.AddSwaggerDoc();
    builder.Services.Configure<DBConfig>(builder.Configuration.GetSection("DBConfig"));
    builder.Configuration.AddEnvironmentVariables(prefix: "PG_");
    builder.Services.ConfigureAWSS3(builder.Configuration);

    builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<BackupJob>(j => j.WithIdentity("Backup").StoreDurably());
            q.AddJob<UploadJob>(j => j.WithIdentity("Upload").StoreDurably());

            q.AddTrigger(t => t.WithIdentity("Backup")
                .StartNow()
                .ForJob("Backup")
                .WithCronSchedule(builder.Configuration.GetSection("Backup").GetValue<string>("Cron") ?? "0 30 3 ? * SUN")
            );
        });

    builder.Services.AddQuartzServer(options =>
    {
        options.AwaitApplicationStarted = true;
        options.WaitForJobsToComplete = true;
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseDefaultExceptionHandler();
    app.UseCors("AllowedOrigins");

    app.UseFastEndpoints();
    app.UseSwaggerGen(_ => { }, c => c.Path = string.Empty);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}