using A2A;
using A2A.AspNetCore;

namespace AgentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTimeOffset.UtcNow }));

            var taskManager = new TaskManager();
            var holidayAgent = new HolidayAgent();
            holidayAgent.Attach(taskManager);
            
            app.MapA2A(taskManager, "/holiday");
            app.MapWellKnownAgentCard(taskManager, "/holiday");
            app.MapHttpA2A(taskManager, "/holiday");

            app.Run();
        }
    }
}
