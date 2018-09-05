using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace WebConsoleDemo.Server.Hubs {
    public class AgentHub : Hub {
        private readonly ILogger<AgentHub> _logger;
        private readonly IHubContext<WebConsoleHub> _webConsoleContext;

        public AgentHub(ILogger<AgentHub> logger, IHubContext<WebConsoleHub> webConsoleContext) {
            _logger = logger;
            _webConsoleContext = webConsoleContext;
        }

        public Task Output(string message, bool isError) {
            _logger.LogInformation($"Received output {message} from agent (error: {isError})");

            // forward agent's output to web consoles
            return _webConsoleContext.Clients.All.SendAsync("output", message, isError);
        }
    }
}