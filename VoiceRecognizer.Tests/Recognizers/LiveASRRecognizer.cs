using System;
using System.Diagnostics;
using System.Net.WebSockets;
using VoiceRecognizer.Tests.Websockets;
using WebSocketSharp.Server;

namespace VoiceRecognizer.Tests.Recognizers
{
    public class LiveASRRecognizer : ISpeechRecognizer
    {
        public bool enabled { get; set; }
        public bool useConfidence { get; set; }
        public float confidence { get; set; }
        public string? cloudVarPath { get; set; }
        public string latestPhrase { get; set; }

        // public string cmd = @"C:\Python310\python.exe";
        public string cmd = "python";
        public string args = @"D:\Mods\Tests\LiveASR\live_asr.py";
        private Process? process;
        private NetworkClass nc;
        private bool isReady = false;

        public LiveASRRecognizer()
        {
            this.enabled = true;
            this.useConfidence = false;
            this.confidence = 0.75f;
            this.cloudVarPath = null;
            this.latestPhrase = string.Empty;
        }
        
        public LiveASRRecognizer(bool enabled, bool useConfidence, float confidence, string? cloudVarPath)
        {
            this.enabled = enabled;
            this.useConfidence = useConfidence;
            this.confidence = confidence;
            this.cloudVarPath = cloudVarPath;
            this.latestPhrase = string.Empty;
        }

        public void Initialize()
        {
            nc = new NetworkClass();
            run_cmd(cmd, args);
            Console.WriteLine($"[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "Ready");
        }

        private void run_cmd(string cmd, string args)
        {
            process = new Process();
            process.StartInfo.FileName = cmd;
            process.StartInfo.Arguments = string.Join(" ", new string[] { "-u", args });
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.OutputDataReceived += OnOutput;
            process.ErrorDataReceived += OnError;
            process.Exited += OnClose;
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        private void OnClose(object? sender, EventArgs e)
        {
            Console.WriteLine($"[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "Closed");
            nc.Stop();
            process?.Close();
        }

        private void OnOutput(object sender, DataReceivedEventArgs e)
        {
            if (isReady)
            {
                // Send a websocket message to the client
                latestPhrase = e.Data ?? string.Empty;
                nc.buffer = latestPhrase;
                nc.broadcastData();
                Console.WriteLine(latestPhrase);
                Console.WriteLine("");
            }
            else if (e.Data == "listening to your voice")
            {
                isReady = true;
            }
        }

        private void OnError(object sender, DataReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[" + DateTime.Now.ToString("HH:mm:ss") + "] " + "Error from LiveASR: " + e.Data);
            Console.ResetColor();
        }

        public void Update() { }

        public void Teardown()
        {
            nc.Stop();
            process?.Close();
        }
    }
}
