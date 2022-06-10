global using FastEndpoints;
using FastEndpoints.Swagger;
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
    builder.Services.AddFastEndpoints();
    builder.Services.AddSwaggerDoc();

    builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<TestJob>(j => j.WithIdentity("Test").StoreDurably());

            q.AddTrigger(t => t.StartNow().ForJob("Test"));
        });

    builder.Services.AddQuartzServer(options =>
    {
        options.AwaitApplicationStarted = true;
        options.WaitForJobsToComplete = true;
    });

    builder.Services.AddSingleton<TestJob>();

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