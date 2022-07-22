using FrooxEngine;
using CloudX.Shared;
using HarmonyLib;
using NeosModLoader;
using System;
using System.Globalization;
using System.Speech.Recognition;

namespace VoiceCommands
{
    public class VoiceCommands : NeosMod
    {
        public override string Name => "VoiceCommands";
        public override string Author => "dfgHiatus";
        public override string Version => "1.0.0";
        public override string Link => "hhttps://github.com/dfgHiatus/NeosVoiceRecognition";

        public static ModConfiguration config;
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> enabled = new ModConfigurationKey<bool>("enabled", "Use Voice Recognition", () => true);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> useConfidence = new ModConfigurationKey<bool>("useConfidence", "Use Voice Confidence threshold", () => false);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<float> confidence = new ModConfigurationKey<float>("confidence", "Confidence threshold", () => 0.75f);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<string> cloudVarPath = new ModConfigurationKey<string>("cloud_Var_Path", "Cloud variable path to write to", () => "speech-recognition.string");

        //
        // Creating your own "speech-recognition.string" cloud variable needs 4 commands sent to the Neos Bot
        // https://wiki.neos.com/Cloud_Variables#User_Color
        // 
        // createUserVar speech-recognition.string
        // setUserVarType speech-recognition.string string
        // setUserVarPerms speech-recognition.string read variable_owner_unsafe
        // setUserVarPerms speech-recognition.string write variable_owner
        //

        private static CloudVariableManager cloudVariableManager;
        private static CloudVariableIdentity cloudVariableIdentity;
        private static CloudVariableProxy cloudVariableProxy;
        private static SpeechRecognitionEngine recognizer;

        public override void OnEngineInit()
        {
            if (Engine.Current.Platform != Platform.Windows)
            {
                Warn("Voice recognition is currently unavailable for non-Windows operating systems");
            }

            Engine.Current.RunPostInit(() => InitSpeechRecognizer());
            Engine.Current.RunPostInit(() => InitCloudInterface());
            Engine.Current.LocalesUpdated += UpdateLocale;
            config = GetConfiguration();
            config.OnThisConfigurationChanged += UpdateCloudPath;
            new Harmony("net.dfgHiatus.Template").PatchAll();
        }

        private void InitSpeechRecognizer()
        {
            var locale = Settings.ReadValue<string>("Interface.Locale", null) ?? "en-US";
            Debug($"Starting speech recognition for {Engine.Current.GetLocaleNativeName(locale)}. Loaded {locale} locale");

            using (recognizer = new SpeechRecognitionEngine(new CultureInfo(locale)))
            {
                recognizer.LoadGrammar(new DictationGrammar());
                recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognized);
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void InitCloudInterface()
        {
            cloudVariableManager = new CloudVariableManager(Engine.Current.Cloud);
            cloudVariableIdentity = new CloudVariableIdentity(Engine.Current.Cloud.CurrentUser.Id, config.GetValue(cloudVarPath));
            cloudVariableProxy = new CloudVariableProxy(cloudVariableIdentity, cloudVariableManager);
        }

        private void UpdateCloudPath(ConfigurationChangedEvent configurationChangedEvent)
        {
            if (configurationChangedEvent.Key.Name == cloudVarPath.Name) // "cloud_Var_Path"
            {
                cloudVariableIdentity = new CloudVariableIdentity(Engine.Current.Cloud.CurrentUser.Id, config.GetValue(cloudVarPath));
                // cloudVariableProxy = new CloudVariableProxy(cloudVariableIdentity, cloudVariableManager); ?
            }
        }

        private void UpdateLocale()
        {
            var newLocale = Settings.ReadValue<string>("Interface.Locale", null) ?? "en-US";
            Debug($"Changing speech recognition for {Engine.Current.GetLocaleNativeName(newLocale)}. Loaded {newLocale} locale");
            recognizer = new SpeechRecognitionEngine(new CultureInfo(newLocale)); // TODO See if we need to pass a body into this
        }

        private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Debug("Recognized text: " + e.Result.Text);
            
            if (config.GetValue(useConfidence))
            {
                if (e.Result.Confidence >= config.GetValue(confidence))
                {
                    cloudVariableProxy.WriteToCloud();
                }
            }
            else
            {
                cloudVariableProxy.WriteToCloud();
            }
        }
    }
}