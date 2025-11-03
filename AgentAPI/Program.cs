using A2A;
using A2A.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using AgentAPI.Services;

namespace AgentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders().AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
            builder.Services.AddLogging();

            builder.Services.AddSingleton<HolidayAgent>();
            builder.Services.AddSingleton<ITaskManager, TaskManager>();

            builder.Services.AddHttpClient<ApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://date.nager.at/");
            });

            var app = builder.Build();


            var taskManager = app.Services.GetRequiredService<ITaskManager>();
            var holidayAgent = app.Services.GetRequiredService<HolidayAgent>();
            holidayAgent.Attach(taskManager);

            app.MapA2A(taskManager, "/holiday");
            app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTimeOffset.UtcNow }));
            app.MapGet("/.well-known/agent.json", async (
                [FromServices] ITaskManager manager,
                HttpContext context,
                CancellationToken cancellationToken) =>
            {
                var agentUri = $"{context.Request.Scheme}://{context.Request.Host}/holiday";
                var agentCard = manager.OnAgentCardQuery.Invoke(agentUri, cancellationToken);

                var options = new JsonSerializerOptions { WriteIndented = true };
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(agentCard.Result, options));
            });

            app.Run();
        }
    }
}
