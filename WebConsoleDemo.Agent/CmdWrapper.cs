using System;
using System.Diagnostics;

namespace WebConsoleDemo.Agent {
    public class CmdWrapper {
        private Process _process;

        public bool Started { get; private set; }

        public void Start(Action<string> onStdout, Action<string> onStderr) {
            var processStartInfo = new ProcessStartInfo("cmd.exe");

            processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            processStartInfo.CreateNoWindow = true;

            processStartInfo.UseShellExecute = false;

            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardError = true;

            _process = new Process();
            _process.StartInfo = processStartInfo;

            _process.OutputDataReceived += (_, args) => {
                if (args.Data != null) {
                    onStdout(args.Data);
                } else {
                    _process = null;
                }
            };
            _process.ErrorDataReceived += (_, args) => {
                if (args.Data != null) {
                    onStderr(args.Data);
                } else {
                    _process = null;
                }
            };

            Started = _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
        }

        public void Kill() {
            _process?.Kill();
            _process = null;
        }

        public void Write(string s) {
            _process.StandardInput.WriteLine(s);
        }
    }
}