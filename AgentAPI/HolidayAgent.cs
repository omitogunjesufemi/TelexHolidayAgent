using A2A;

namespace AgentAPI
{
    public class HolidayAgent
    {
        public void Attach(ITaskManager taskManager)
        {
            taskManager.OnMessageReceived = ProcessMessageAsync;
            taskManager.OnAgentCardQuery = GetAgentCardAsync;
        }

        private Task<A2AResponse> ProcessMessageAsync(MessageSendParams messageSendParams, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<A2AResponse>(cancellationToken);

            var messageText = messageSendParams.Message.Parts.OfType<TextPart>().First().Text;

            var responseText = $"Agent received: '{messageText}'. Working on your holiday request...";

            var message = new AgentMessage()
            {
                Role = MessageRole.Agent,
                MessageId = Guid.NewGuid().ToString(),
                ContextId = messageSendParams.Message.ContextId,
                Parts = [
                    new TextPart()
                    {
                        Text = $"Echo: {messageText}"
                    }
                ]
            };

            return Task.FromResult<A2AResponse>(message);
        }

        private Task<AgentCard> GetAgentCardAsync(string agentUrl, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<AgentCard>(cancellationToken);

            var capabilities = new AgentCapabilities()
            {
                Streaming = true,
                PushNotifications = false,
            };

            return Task.FromResult(new AgentCard()
            {
                Name = "Holiday Agent",
                Description = "The agent ensures teams in different regions are aware of upcoming holidays",
                Url = agentUrl,
                Version = "1.0.0",
                DefaultInputModes = ["text"],
                DefaultOutputModes = ["text"],
                Capabilities = capabilities,
                Skills = []
            });
        }
    }
}
