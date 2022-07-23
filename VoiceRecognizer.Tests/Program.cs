using System;
using System.Speech.Recognition;
using VoiceRecognizer.Tests;
using VoiceRecognizer.Tests.LiveASR;
using VoiceRecognizer.Tests.System;
using VoiceRecognizer.Tests.Windows;

namespace SampleRecognition
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Not working. In addition to needing Core 3.1 (.NET 5+ is NOT an option), this requires a WPF, not cross platform.
            // var winSpeech = new WindowsMediaRecognizer();
            // winSpeech.Initialize();

            // Shoddy. According to SO, this hasn't been updated since Vista/7. Also not cross platform.
            // var systemSpeech = new SystemSpeechRecognizer();
            // systemSpeech.Initialize();

            // Gaming
            var liveASR = new LiveASRRecognizer();
            liveASR.Initialize();
        }
    }
}