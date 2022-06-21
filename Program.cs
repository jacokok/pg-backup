global using FastEndpoints;
using FastEndpoints.Swagger;
using PGBackup;
using PGBackup.Extensions;
using PGBackup.Jobs;
using Quartz;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("Starting up");

try
{
    Directory.CreateDirectory("backup");
    builder.Services.AddFastEndpoints();
    builder.Services.AddSwaggerDoc();
    builder.Services.Configure<DBConfig>(builder.Configuration.GetSection("DBConfig"));
    builder.Configuration.AddEnvironmentVariables(prefix: "PG_");
    builder.Services.ConfigureAWSS3(builder.Configuration);

    builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<BackupJob>(j => j.WithIdentity("Backup").StoreDurably());
            q.AddJob<TestJob>(j => j.WithIdentity("Test").StoreDurably());
            q.AddJob<UploadJob>(j => j.WithIdentity("Upload").StoreDurably());

            // q.AddTrigger(t => t.WithIdentity("TestMonthly").StartNow().WithSimpleSchedule(s => s.RepeatForever().WithIntervalInMinutes(1)).ForJob("Backup"));
        });

    builder.Services.AddQuartzServer(options =>
    {
        options.AwaitApplicationStarted = true;
        options.WaitForJobsToComplete = true;
    });

    // builder.Services.AddSingleton<TestJob>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseDefaultExceptionHandler();

    app.UseFastEndpoints();
    app.UseOpenApi();
    app.UseSwaggerUi3(s =>
       {
           s.ConfigureDefaults();
           s.Path = string.Empty;
       });
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