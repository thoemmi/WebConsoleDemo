using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace WebConsoleDemo.Agent {
    internal class Program {
        private static async Task Main(string[] args) {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/console")
                .Build();

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            connection.Closed += e => {
                cts.Cancel();
                return Task.CompletedTask;
            };

            var cmd = new CmdWrapper();

            connection.On<string>("Stdout", _ => { });
            connection.On<string>("Stderr", _ => { });
            connection.On<string>("Stdin", s => {
                if (!cmd.Started) {
                    cmd.Start(
                        stdout => connection.InvokeAsync("Stdout", stdout).GetAwaiter().GetResult(),
                        stderr => connection.InvokeAsync("Stderr", stderr).GetAwaiter().GetResult()
                    );
                }
                cmd.Write(s);
            });

            await connection.StartAsync();

            Console.WriteLine("Press Ctrl+C to exit");
            cts.Token.WaitHandle.WaitOne();

            await connection.DisposeAsync();

            cts.Dispose();
        }
    }
}