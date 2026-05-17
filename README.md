# FishS2Sharp
A proper C# wrapper for the **Fish Audio S2 Pro** repo [s2.cpp](https://github.com/rodrigomatta/s2.cpp).

What features are currently implemented:
- Ability to transform text to voice using either ***only using a text input*** and/or combined ***with an reference audio(voice sample) file***.
- Ability to save the resulting audio to ***file*** and/or retreive it ***as an array of float samples*** in code(mono).
- The library is modular, meaning you can load the model/clients separately, which allows you to re-use the model in other FishS2Client instances without reloading them again.
- Audio references are binded and stored in FishAudioVoiceReference instances, and they are processed at the time you create/add/register them with RegisterVoiceReference(). This allows to process the reference voice samples for the model ***ONLY ONCE*** and re-use it without having to re-load it for every prompt. (NOTE: You can only use/synth. processed voice samples for the model you used to process them with.)
- CUDA, Vulkan and Metal support, as well as CPU fallback. (As long you have **ggml-cuda.dll**/**ggml-vulkan.dll**/**ggml-metal.dll** in the same folder as this library and the **s2.dll**)
- This library is compiled against the netstandard 2.1, that means you can use it both in .NET 5+ applications and the Unity game engine, allowing this to be used for games as well.

You still have to build [s2.dll](https://github.com/rodrigomatta/s2.cpp) manually along with the ggml.

Occasionally I'll include pre-built versions of S2 along with the ggml dll's in [Releases](https://github.com/subspecs/FishS2Sharp/releases/), but usually they'll only come with CUDA/VULKAN/CPU support since I don't use crapple(apple).


## Model variants

GGUF files are available at [rodrigomt/s2-pro-gguf](https://huggingface.co/rodrigomt/s2-pro-gguf) on Hugging Face.

| File | Size | Notes |
|---|---|---|
| `s2-pro-f16.gguf` | 9.9 GB | Full precision — reference quality |
| `s2-pro-q8_0.gguf` | 5.6 GB | Near-lossless — model weights use ~5 GB VRAM after the duplicate-load fix; full runtime usage is higher |
| `s2-pro-q6_k.gguf` | 4.5 GB | Good quality/size balance — recommended for 6+ GB VRAM |
| `s2-pro-q5_k_m.gguf` | 4.0 GB | Smaller with still-good quality |
| `s2-pro-q4_k_m.gguf` | 3.6 GB | Best compact variant so far in quick RU validation |
| `s2-pro-q3_k.gguf` | 3.0 GB | Usable, but starts stretching short words |
| `s2-pro-q2_k.gguf` | 2.6 GB | Lowest-size experimental variant |

All variants include both the transformer weights and the audio codec in a single file.
The quantized variants above were regenerated with the codec tensors (`c.*`) kept in `F16`, so only the AR transformer is quantized.

Example usage:
```C#
internal class Program
{
    static void Main(string[] args)
    {
            string ModelFileName = "s2-pro-q4_k_m.gguf";
            string ModelFolder = "D:\\AI Models\\FishModels\\";
            string VoiceFolderDir = "";


            System.Console.WriteLine("Loading Model...");

            //First we load a shared model instance. This instance can be re-used in multiple FishS2Client instances.
            FishS2Sharp.FishModel SharedModel = new FishS2Sharp.FishModel(ModelFolder + ModelFileName, ModelFolder + "tokenizer.json", FishS2Sharp.GPUBackendTypes.Cuda);

            System.Console.WriteLine("Done Loading Model!");


            System.Console.WriteLine("Generating Voice...");

            //You can create re-usable instances of cloned voices. All you need is a 10-15s voice sample, and you need a transcript of what is spoken in the voice sample.
            //Just don't mix/match different VoiceReference's generated from one type of model and run inference/synthesize with another. 
            FishS2Sharp.FishAudioVoiceReference VoiceReference = new FishS2Sharp.FishAudioVoiceReference(SharedModel, "Mortal Combat Voice", VoiceFolderDir + "2.mp3",
                "Raiden! Shang Tsung! Kitana! Choose your destiny! Johnny Cage! Sonya Blade! Kano! Jax! Round One... FIGHT! Finish Him! FATALITY! Flawless Victory!");

            System.Console.WriteLine("Done Generating Voice!");


            //Create an FishS2Client instance. These are NOT thread safe, so you should use your own locking mechanism.
            //If you need multithreaded processing, use a new FishS2Client instance per thread.
            FishS2Sharp.FishS2Client Instance = new FishS2Sharp.FishS2Client(SharedModel);

            //Create default pipeline settings. You can change settings inside it like TopK/P, Temp, MaxTokens etc..
            FishS2Sharp.FishAudioParameters PipelineParameters = new FishS2Sharp.FishAudioParameters() { Temp = 0.8f };


            //Finally, we synthesize our chosen text to our specific(but optional) voice:
            System.Console.WriteLine("Generating TTS...");

            System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch(); Timer.Start();
            Instance.Synthesize("My name is Jeff and england is my city!", "D:\\Jeff.wav", PipelineParameters, VoiceReference);

            Timer.Stop(); System.Console.WriteLine("Generation Time: " + Timer.Elapsed.TotalSeconds.ToString("0.000") + "s"); Timer.Reset();

            //Cleanup this sample code.
            Instance.Dispose();
    }
}
```
### **Want to [help me](https://www.patreon.com/subspecs) pay off my therapist? Might make updates more frequent!**
