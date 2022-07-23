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
            // Not working. In addition to needing Core 3.1, this requires a WPF, not cross platform.
            // var winSpeech = new WindowsMediaRecognizer();
            // winSpeech.Initiallize();

            // Shoddy. According to SO, this hasn't been updated since Vista/7. Also not cross platform.
            var systemSpeech = new SystemSpeechRecognizer();
            systemSpeech.Initiallize();
        }
    }
}