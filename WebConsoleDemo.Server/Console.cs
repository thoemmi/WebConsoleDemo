using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace WebConsoleDemo.Server {
    public class Console : Hub {
        private readonly ILogger<Console> _logger;

        public Console(ILogger<Console> logger) {
            _logger = logger;
        }

        public Task Stdout(string s) {
            System.Console.WriteLine($"Stdout: {s}");
            _logger.LogInformation($"Stdout: {s}");

            return Clients.All.SendAsync("Stdout", s);
        }

        public Task Stderr(string s) {
            System.Console.WriteLine($"Stderr: {s}");
            _logger.LogWarning($"Stderr: {s}");

            return Clients.All.SendAsync("Stderr", s);
        }

        public Task Stdin(string s) {
            System.Console.WriteLine($"Stdin: {s}");
            _logger.LogInformation($"Stdin: {s}");

            return Clients.All.SendAsync("Stdin", s);
        }
    }
}