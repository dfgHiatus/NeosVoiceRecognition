using System;
using System.Speech.Recognition;

namespace SampleRecognition
{
    class Program
    {
        static void Main(string[] args)

        // Initialize an in-process speech recognition engine.  
        {
            using (SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine())
            {

                // Create SemanticResultValue objects that contain cities and airport codes.  
                SemanticResultValue chicago = new SemanticResultValue("Chicago", "ORD");
                SemanticResultValue boston = new SemanticResultValue("Boston", "BOS");
                SemanticResultValue miami = new SemanticResultValue("Miami", "MIA");
                SemanticResultValue dallas = new SemanticResultValue("Dallas", "DFW");

                // Create a Choices object and add the SemanticResultValue objects, using  
                // implicit conversion from SemanticResultValue to GrammarBuilder  
                Choices cities = new Choices();
                cities.Add(new Choices(new GrammarBuilder[] { chicago, boston, miami, dallas }));

                // Build the phrase and add SemanticResultKeys.  
                GrammarBuilder chooseCities = new GrammarBuilder();
                chooseCities.Append("I want to fly from");
                chooseCities.Append(new SemanticResultKey("origin", cities));
                chooseCities.Append("to");
                chooseCities.Append(new SemanticResultKey("destination", cities));

                // Build a Grammar object from the GrammarBuilder.  
                Grammar bookFlight = new Grammar(chooseCities);
                bookFlight.Name = "Book Flight";

                // Add a handler for the LoadGrammarCompleted event.  
                recognizer.LoadGrammarCompleted +=
                  new EventHandler<LoadGrammarCompletedEventArgs>(recognizer_LoadGrammarCompleted);

                // Add a handler for the SpeechRecognized event.  
                recognizer.SpeechRecognized +=
                  new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

                // Load the grammar object to the recognizer.  
                recognizer.LoadGrammarAsync(bookFlight);

                // Set the input to the recognizer.  
                recognizer.SetInputToDefaultAudioDevice();

                // Start recognition.  
                recognizer.RecognizeAsync();

                // Keep the console window open.  
                Console.ReadLine();
            }
        }

        // Handle the LoadGrammarCompleted event.  
        static void recognizer_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            Console.WriteLine("Grammar loaded: " + e.Grammar.Name);
            Console.WriteLine();
        }

        // Handle the SpeechRecognized event.  
        static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Speech recognized:  " + e.Result.Text);
            Console.WriteLine();
            Console.WriteLine("Semantic results:");
            Console.WriteLine("  The flight origin is " + e.Result.Semantics["origin"].Value);
            Console.WriteLine("  The flight destination is " + e.Result.Semantics["destination"].Value);
        }
    }
}