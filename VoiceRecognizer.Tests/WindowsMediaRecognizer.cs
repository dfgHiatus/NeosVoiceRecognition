using System;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace VoiceRecognizer.Tests.Windows
{
    public class WindowsMediaRecognizer : ISpeechRecognizer
    {
        public bool enabled { get; set; }
        public bool useConfidence { get; set; }
        public float confidence { get; set; }
        public string? cloudVarPath { get; set; }

        public WindowsMediaRecognizer()
        {
            enabled = true;
            useConfidence = false;
            confidence = 0.75f;
            cloudVarPath = null;
        }

        public WindowsMediaRecognizer(bool enabled, bool useConfidence, float confidence, string? cloudVarPath)
        {
            this.enabled = enabled;
            this.useConfidence = useConfidence;
            this.confidence = confidence;
            this.cloudVarPath = cloudVarPath;
        }

        public void Initiallize()
        {
            Task.Run(async () =>
            {
                try
                {
                    var speech = new SpeechRecognizer();
                    Console.WriteLine("Setup recognizer.");
                    await speech.CompileConstraintsAsync();
                    Console.WriteLine("Setup compiled.");
                    SpeechRecognitionResult result = await speech.RecognizeAsync();
                    Console.WriteLine(result.Text);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        public void Teardown()
        {
            
        }
    }
}
