using System;
using System.Speech.Recognition;
using VoiceRecognizer.Tests.Websockets;

namespace VoiceRecognizer.Tests.Recognizers
{
    public class SystemSpeechRecognizer : ISpeechRecognizer
    {
        public bool enabled { get; set; }
        public bool useConfidence { get; set; }
        public float confidence { get; set; }
        public string? cloudVarPath { get; set; }
        public string latestPhrase { get; set; }

        private NetworkClass nc;

        public SystemSpeechRecognizer()
        {
            enabled = true;
            useConfidence = false;
            confidence = 0.75f;
            cloudVarPath = null;
            latestPhrase = string.Empty;
        }

        public SystemSpeechRecognizer(bool enabled, bool useConfidence, float confidence, string? cloudVarPath)
        {
            this.enabled = enabled;
            this.useConfidence = useConfidence;
            this.confidence = confidence;
            this.cloudVarPath = cloudVarPath;
            latestPhrase = string.Empty;
        }

        public void Initialize()
        {
            nc = new NetworkClass();
            using ( SpeechRecognitionEngine recognizer =  new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US")))
            {
                // Create and load a dictation grammar.  
                recognizer.LoadGrammar(new DictationGrammar());

                // Add a handler for the speech recognized event.  
                recognizer.SpeechRecognized +=
                    new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

                // Configure input to the speech recognizer.  
                recognizer.SetInputToDefaultAudioDevice();

                // Start asynchronous, continuous speech recognition.  
                recognizer.RecognizeAsync(RecognizeMode.Multiple);

                // Keep the console window open.  
                while (true)
                {
                    Console.ReadLine();
                }
            }
        }

        private void recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            latestPhrase = e.Result.Text;
            nc.buffer = latestPhrase;
            nc.broadcastData();
            Console.WriteLine(latestPhrase);
            Console.WriteLine("");
        }

        public void Update() { }

        public void Teardown() => nc.Stop();
    }
}
