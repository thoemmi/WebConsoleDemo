using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace WebConsoleDemo.Agent {
    internal class Program {
        private static async Task Main(string[] args) {
            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/agent")
                .Build();

            connection.Closed += e => {
                cts.Cancel();
                return Task.CompletedTask;
            };

            var cmd = new CmdWrapper();

            connection.On<string>("execute", s => {
                if (!cmd.Started) {
                    cmd.Start(
                        stdout => {
                            Console.WriteLine(stdout);

                            connection.InvokeAsync("Output", stdout, false).GetAwaiter().GetResult();
                        },
                        stderr => {
                            var oldColor = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(stderr);
                            Console.ForegroundColor = oldColor;

                            connection.InvokeAsync("Output", stderr, true).GetAwaiter().GetResult();
                        });
                }
                cmd.Write(s);
            });

            await connection.StartAsync(cts.Token);

            Console.WriteLine("Press Ctrl+C to exit");
            cts.Token.WaitHandle.WaitOne();

            await connection.DisposeAsync();

            cts.Dispose();
        }
    }
}