using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace WebConsoleDemo.Server.Hubs {
    public class WebConsoleHub : Hub {
        private readonly ILogger<WebConsoleHub> _logger;
        private readonly IHubContext<AgentHub> _agentHubContext;

        public WebConsoleHub(ILogger<WebConsoleHub> logger, IHubContext<AgentHub> agentHubContext) {
            _logger = logger;
            _agentHubContext = agentHubContext;
        }

        public Task SendCommand(string command) {
            _logger.LogInformation($"Received command {command} from web console");

            // forward command to agents
            return _agentHubContext.Clients.All.SendAsync("execute", command);
        }
    }
}