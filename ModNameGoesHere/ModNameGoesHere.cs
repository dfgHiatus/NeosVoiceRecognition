using BaseX;
using CloudX.Shared;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;

namespace ModNameGoesHere
{
    public class ModNameGoesHere : NeosMod
    {
        public override string Name => "ModNameGoesHere";
        public override string Author => "dfgHiatus";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/GithubUsername/RepoName/";

        public static ModConfiguration config;
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> enabled = new ModConfigurationKey<bool>("enabled", "Use Voice Recognition", () => true);
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<string> cloudVarPath = new ModConfigurationKey<string>("cloud_Var_Path", "Cloud Variable Path to write to", () => "speech-recognition.string");

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
            if (configurationChangedEvent.Key.Name == "cloud_Var_Path")
            {
                cloudVariableIdentity = new CloudVariableIdentity(Engine.Current.Cloud.CurrentUser.Id, config.GetValue(cloudVarPath));
                // cloudVariableProxy = new CloudVariableProxy(cloudVariableIdentity, cloudVariableManager); ?
            }
        }

        private void UpdateLocale()
        {
            var newLocale = Settings.ReadValue<string>("Interface.Locale", null) ?? "en-US";
            Debug($"Changing speech recognition for {Engine.Current.GetLocaleNativeName(newLocale)}. Loaded {newLocale} locale");
            // TODO See if we need to pass a body into this
            recognizer = new SpeechRecognitionEngine(new CultureInfo(newLocale));
        }

        private static void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Debug("Recognized text: " + e.Result.Text);
            cloudVariableProxy.WriteToCloud();
        }
    }
}