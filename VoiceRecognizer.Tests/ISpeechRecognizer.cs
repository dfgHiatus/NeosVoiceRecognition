namespace VoiceRecognizer.Tests
{
    public interface ISpeechRecognizer
    {
        public bool enabled { get; set; }
        public bool useConfidence { get; set; }
        public float confidence { get; set; }
        public string? cloudVarPath { get; set; }

        public abstract void Initiallize();
        public abstract void Teardown();
    }
}
