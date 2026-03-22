# FishS2Sharp
A proper C# wrapper for the **Fish Audio S2 Pro** repo [s2.cpp](https://github.com/rodrigomatta/s2.cpp).

This is currently in alpha/bleeding edge state.

A nuget will be created overtime.

What features are currently implemented:
- Ability to transform text to voice using either only a text input or with an reference voice.
- Ability to either save the resulting audio to file OR retreive it as an array of float samples in code.
- The library is modular, meaning you can load the model/tokenizer separately, which allows you to re-use the model/tokenizer in other FishS2Client instances without reloading them again.
- Audio references are binded and stored in FishS2Client instances, and they are processed at the time you create/add/register them with RegisterVoiceReference(). This allows to process the reference voice sample for the model once and re-use it without having to re-load it for every prompt.
- CUDA and Vulkan support, as well as CPU fallback. (As long you have **ggml-cuda.dll**/**ggml-vulkan.dll** in the same folder as this library and s2.dll)
- This library is compiled against the netstandard 2.1, that means you can use it both in .NET 5+ applications and the Unity game engine, allowing this to be used for games as well.

Example usage:
```C#
internal class Program
{
    static void Main(string[] args)
    {
        const string SomeLocation = @"C:\Users\SubSpecs\Desktop\s2.cpp-main\build\bin\RelWithDebInfo\";

        //Create an FishS2Client instance. (You can have as many as you want, models/tokenizers can be shared across instances)
        FishS2Sharp.FishS2Client Instance = new FishS2Sharp.FishS2Client(SomeLocation + "s2 - pro-q8_0.gguf", SomeLocation + "tokenizer.json", FishS2Sharp.GPUBackendTypes.Cuda);

        //(Optional) Register a reference voice with some transcript of what is said in said sample. Wav/mp3's currently onyl supported. (10-20s samples recommended.)
        Instance.RegisterVoiceReference("Mortal Combat", SomeLocation + "2.mp3", "Raiden! Shang Tsung! Kitana! Choose your destiny! Johnny Cage! Sonya Blade! Kano! Jax! " +
                "Round One... FIGHT! Finish Him! FATALITY! Flawless Victory!", out _);

        //Create some pipeline settings.
        FishS2Sharp.FishAudioParameters PipelineParameters = new FishS2Sharp.FishAudioParameters(/*int max_new_tokens = -1, float temperature = -1, float top_p = -1...*/);

        //Synthesize text to voice:
        System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch(); Timer.Start();
        Instance.Synthesize("My name is Jeff and england is my city!", "D:\\Jeff.wav", PipelineParameters, Instance.GetVoiceReference("Mortal Combat"));
        Timer.Stop(); System.Console.WriteLine("Generation Time: " + Timer.Elapsed.TotalSeconds.ToString("0.000") + "s");

        //Cleanup this sample code.
        Instance.Dispose();
    }
}
```
### **Want to [help me](https://www.patreon.com/subspecs) pay off my therapist? Might make updates more frequent!**
