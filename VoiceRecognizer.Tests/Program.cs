using System;
using System.Speech.Recognition;
using VoiceRecognizer.Tests;
using VoiceRecognizer.Tests.System;
using VoiceRecognizer.Tests.Windows;

namespace SampleRecognition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var winSpeech = new WindowsMediaRecognizer();
            winSpeech.Initiallize();

            // var systemSpeech = new SystemSpeechRecognizer();
            // systemSpeech.Initiallize();
        }
    }
}