namespace VoiceRecognizer.Tests
{
    public interface ISpeechRecognizer
    {
        public bool enabled { get; set; }
        public bool useConfidence { get; set; }
        public float confidence { get; set; }
        public string? cloudVarPath { get; set; }
        public string latestPhrase { get; set; }

        public abstract void Initialize();
        public abstract void Update();
        public abstract void Teardown();
    }
}
