namespace FishS2Sharp
{
    public enum GPUBackendTypes
    {
        CPU = -1,
        Vulkan = 0,
        Cuda = 1
    }

    internal static class Native
    {
        const string DllPath = @"s2.dll";

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2Pipeline();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2Pipeline(void* Pipeline);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void SyncS2TokenizerConfigFromS2Model(void* Model, void* Tokenizer);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2Pipeline(void* Pipeline, void* Tokenizer, void* Model, void* AudioCodec);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2GenerateParams();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2GenerateParams(void* GenerateParams);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2GenerateParams(void* GenerateParams, int max_new_tokens = -1, float temperature = -1, float top_p = -1, int top_k = -1, int min_tokens_before_end = -1, int n_threads = -1, int verbose = -1);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2Model();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2Model(void* Model);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2Model(void* Model, string gguf_path, int gpu_device, int backend_type);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2Tokenizer();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2Tokenizer(void* Tokenizer);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2Tokenizer(void* Tokenizer, string path);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2AudioCodec();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2AudioCodec(void* AudioCodec);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2AudioCodec(void* AudioCodec, string gguf_path, int gpu_device, int backend_type);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2AudioPromptCodes();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2AudioPromptCodes(void* AudioPromptCodes);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeAudioPromptCodes(void* Pipeline, int ThreadCount, string ReferenceAudioPath, void* AudioPromptCodes, int* TPrompt);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2AudioBuffer(int InitialSize = -1);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2AudioBuffer(void* AudioBuffer);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern float* GetS2AudioBufferDataPointer(void* AudioBuffer);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int S2Synthesize(void* Pipeline, void* GenerateParams, void* AudioBuffer, void* ReferenceAudioPromptCodes, int* ReferenceAudioTPrompt, string ReferenceAudioPath = null, string ReferenceAudioTranscript = "", string TextToInfer = "", string OutputAudioPath = null, int* AudioBufferOutputLength = null);
    }

    public unsafe class FishModel
    {
        public int DeviceID { get { return _DeviceID; } }
        public GPUBackendTypes Backend { get { return _BackendType; } }
        public string ModelPath { get { return _ModelPath; } }

        internal long _ModelID;
        private int _DeviceID;
        private bool IsDisposed;
        private string _ModelPath;
        internal void* ModelHandle;
        private GPUBackendTypes _BackendType;

        //Constructors:
        public FishModel(string ModelPath, GPUBackendTypes BackendType, int DeviceID = 0)
        {
            _ModelPath = ModelPath; _BackendType = BackendType; _DeviceID = (BackendType == GPUBackendTypes.CPU ? -1 : DeviceID);

            ModelHandle = Native.AllocS2Model();
            if (Native.InitializeS2Model(ModelHandle, ModelPath, _DeviceID, (int)BackendType) != 1)
            {
                throw new System.Exception("Failed to initialize model with " + BackendType.ToString() + " support.");
            }
        }
        ~FishModel()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Native.ReleaseS2Model(ModelHandle);
                IsDisposed = true;
            }
        }
    }
    public unsafe class FishTokenizer
    {
        public string TokenizerPath { get { return _TokenizerPath; } }

        private bool IsDisposed;
        private string _TokenizerPath;
        internal void* TokenizerHandle;

        //Constructors:
        public FishTokenizer(string TokenizerPath)
        {
            _TokenizerPath = TokenizerPath;

            TokenizerHandle = Native.AllocS2Tokenizer();
            if (Native.InitializeS2Tokenizer(TokenizerHandle, _TokenizerPath) != 1)
            {
                throw new System.Exception("Failed to initialize tokenizer with path '" + _TokenizerPath + "'.");
            }
        }
        ~FishTokenizer()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Native.ReleaseS2Tokenizer(TokenizerHandle);
                IsDisposed = true;
            }
        }
    }
    public unsafe class FishAudioCodec
    {
        public int DeviceID { get { return _DeviceID; } }
        public GPUBackendTypes Backend { get { return _BackendType; } }
        public string ModelPath { get { return _ModelPath; } }

        private int _DeviceID;
        private bool IsDisposed;
        private string _ModelPath;
        internal void* AudioCodecHandle;
        private GPUBackendTypes _BackendType;

        //Constructors:
        public FishAudioCodec(string ModelPath, GPUBackendTypes BackendType, int DeviceID = 0)
        {
            _ModelPath = ModelPath; _BackendType = BackendType; _DeviceID = (BackendType == GPUBackendTypes.CPU ? -1 : DeviceID);

            AudioCodecHandle = Native.AllocS2AudioCodec();
            if (Native.InitializeS2AudioCodec(AudioCodecHandle, ModelPath, _DeviceID, (int)BackendType) != 1)
            {
                throw new System.Exception("Failed to initialize AudioCodec with " + BackendType.ToString() + " support.");
            }
        }
        ~FishAudioCodec()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Native.ReleaseS2AudioCodec(AudioCodecHandle);
                IsDisposed = true;
            }
        }
    }
    public unsafe class FishAudioBuffer
    {
        private bool IsDisposed;
        internal void* AudioBufferHandle;

        //Constructors:
        public FishAudioBuffer()
        {
            AudioBufferHandle = Native.AllocS2AudioBuffer();
        }
        ~FishAudioBuffer()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Native.ReleaseS2AudioBuffer(AudioBufferHandle);
                IsDisposed = true;
            }
        }
        public float* GetRawAudioBuffer()
        {
            return IsDisposed ? null : Native.GetS2AudioBufferDataPointer(AudioBufferHandle);
        }
    }
    public unsafe class FishAudioParameters
    {
        internal void* GenerateParams;
        private bool IsDisposed;

        //Constructor:
        public FishAudioParameters(int max_new_tokens = -1, float temperature = -1, float top_p = -1, int top_k = -1, int min_tokens_before_end = -1, int n_threads = -1, int verbose = -1)
        {
            GenerateParams = Native.AllocS2GenerateParams();
            Native.InitializeS2GenerateParams(GenerateParams, max_new_tokens, temperature, top_p, top_k, min_tokens_before_end, n_threads, verbose);
        }
        ~FishAudioParameters()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Native.ReleaseS2GenerateParams(GenerateParams);
                IsDisposed = true;
            }
        }
    }

    public unsafe class FishS2Client
    {
        public struct AudioReference
        {
            internal void* Reference;
            internal int TPrompt;
            public string Name, Transcript;
            public AudioReference(string Name, string Transcript, void* Reference, int TPrompt) { this.Name = Name; this.Transcript = Transcript; this.Reference = Reference; this.TPrompt = TPrompt; }
        }

        public FishModel Model { get { return _Model; } }
        public FishTokenizer Tokenizer { get { return _Tokenizer; } }
        public FishAudioCodec AudioCodec { get { return _AudioCodec; } }

        private FishModel _Model;
        private FishTokenizer _Tokenizer;
        private FishAudioCodec _AudioCodec;
        private FishAudioBuffer AudioBuffer;
        private void* Pipeline;
        private bool IsDispose;
        private AudioReference DefaultVoiceReference;
        private System.Collections.Generic.Dictionary<string, AudioReference> AudioReferences;

        //Constructors:
        public FishS2Client(FishModel Model, FishTokenizer Tokenizer)
        {
            _Model = Model;
            _AudioCodec = new FishAudioCodec(_Model.ModelPath, GPUBackendTypes.CPU, -1); //For now.
            _Tokenizer = Tokenizer;
            AudioBuffer = new FishAudioBuffer();
            DefaultVoiceReference.Reference = Native.AllocS2AudioPromptCodes(); DefaultVoiceReference.Transcript = "";
            AudioReferences = new System.Collections.Generic.Dictionary<string, AudioReference>(2);

            Pipeline = Native.AllocS2Pipeline();
            Native.SyncS2TokenizerConfigFromS2Model(_Model.ModelHandle, _Tokenizer.TokenizerHandle);
            if (Native.InitializeS2Pipeline(Pipeline, _Tokenizer.TokenizerHandle, _Model.ModelHandle, _AudioCodec.AudioCodecHandle) != 1)
            {
                throw new System.Exception("Failed to initialize pipeline.");
            }
        }
        public FishS2Client(string ModelPath, string TokenizerPath, GPUBackendTypes BackendType, int DeviceID = 0)
        {
            _Model = new FishModel(ModelPath, BackendType, DeviceID);
            _AudioCodec = new FishAudioCodec(ModelPath, GPUBackendTypes.Cuda, 0); //For now.
            _Tokenizer = new FishTokenizer(TokenizerPath);
            AudioBuffer = new FishAudioBuffer();
            DefaultVoiceReference.Reference = Native.AllocS2AudioPromptCodes(); DefaultVoiceReference.Transcript = "";
            AudioReferences = new System.Collections.Generic.Dictionary<string, AudioReference>(2);

            Pipeline = Native.AllocS2Pipeline();
            Native.SyncS2TokenizerConfigFromS2Model(_Model.ModelHandle, _Tokenizer.TokenizerHandle);
            if (Native.InitializeS2Pipeline(Pipeline, _Tokenizer.TokenizerHandle, _Model.ModelHandle, _AudioCodec.AudioCodecHandle) != 1)
            {
                throw new System.Exception("Failed to initialize pipeline.");
            }
        }
        ~FishS2Client()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose(bool DisposeModel = true, bool DisposeTokenizer = true)
        {
            if (!IsDispose)
            {
                Native.ReleaseS2AudioPromptCodes(DefaultVoiceReference.Reference);
                var Enum = AudioReferences.GetEnumerator();
                while (Enum.MoveNext())
                {
                    Native.ReleaseS2AudioPromptCodes(Enum.Current.Value.Reference);
                }
                Enum.Dispose();
                AudioReferences.Clear();

                Native.ReleaseS2Pipeline(Pipeline);
                AudioBuffer.Dispose();
                _AudioCodec.Dispose();
                if (DisposeTokenizer) { _Tokenizer.Dispose(); }
                if (DisposeModel) { _Model.Dispose(); }
                IsDispose = true;
            }
        }

        public bool RegisterVoiceReference(string ReferenceName, string Mp3WavFileName, string AudioTranscript, out AudioReference Reference)
        {
            AudioReference ARef = new AudioReference(ReferenceName, AudioTranscript, Native.AllocS2AudioPromptCodes(), 0);
            if (Native.InitializeAudioPromptCodes(Pipeline, System.Environment.ProcessorCount, Mp3WavFileName, ARef.Reference, &ARef.TPrompt) != 1)
            {
                throw new System.Exception("Failed to register reference voice.");
            }
            if (!AudioReferences.TryAdd(ReferenceName, ARef))
            {
                Reference = default;
                Native.ReleaseS2AudioPromptCodes(ARef.Reference);
                return false;
            }
            Reference = ARef;
            return true;
        }
        public AudioReference GetVoiceReference(string ReferenceName)
        {
            return AudioReferences[ReferenceName];
        }

        public void Synthesize(string Text, string OutputWavFilename, FishAudioParameters Parameters, AudioReference VoiceReference = default)
        {
            if (VoiceReference.Name == null) { VoiceReference = DefaultVoiceReference; }

            int OutputBufferLength = 0, ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, VoiceReference.Reference, &VoiceReference.TPrompt, null, VoiceReference.Transcript, Text, OutputWavFilename, &OutputBufferLength);

            switch (ErrorCode)
            {
                case 0: { throw new System.Exception("Failed to synthesize pipeline because the pipeline is not initialized."); }
                case -1: { System.Console.WriteLine("[Pipeline Warning]: encode failed, running without reference audio."); } break;
                case -2: { System.Console.WriteLine("[Pipeline Warning]: load_audio failed, running without reference audio."); } break;
                case -3: { throw new System.Exception("[Pipeline Error]: init_kv_cache failed."); }
                case -4: { throw new System.Exception("[Pipeline Error]: generation produced no frames."); }
                case -5: { throw new System.Exception("[Pipeline Error]: decode failed."); }
                case -6: { throw new System.Exception("[Pipeline Error]: save_audio failed."); }
            }
        }
        public int Synthesize(string Text, float* RawAudioOutput, FishAudioParameters Parameters, AudioReference VoiceReference = default)
        {
            if (VoiceReference.Name == null) { VoiceReference = DefaultVoiceReference; }

            int OutputBufferLength = 0, ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, VoiceReference.Reference, &VoiceReference.TPrompt, null, VoiceReference.Transcript, Text, null, &OutputBufferLength);

            switch (ErrorCode)
            {
                case 0: { throw new System.Exception("Failed to synthesize pipeline because the pipeline is not initialized."); }
                case -1: { System.Console.WriteLine("[Pipeline Warning]: encode failed, running without reference audio."); } break;
                case -2: { System.Console.WriteLine("[Pipeline Warning]: load_audio failed, running without reference audio."); } break;
                case -3: { throw new System.Exception("[Pipeline Error]: init_kv_cache failed."); }
                case -4: { throw new System.Exception("[Pipeline Error]: generation produced no frames."); }
                case -5: { throw new System.Exception("[Pipeline Error]: decode failed."); }
                case -6: { throw new System.Exception("[Pipeline Error]: save_audio failed."); }
            }

            RawAudioOutput = AudioBuffer.GetRawAudioBuffer();
            return OutputBufferLength;
        }
        public int Synthesize(string Text, string OutputWavFilename, float* RawAudioOutput, FishAudioParameters Parameters, AudioReference VoiceReference = default)
        {
            if (VoiceReference.Name == null) { VoiceReference = DefaultVoiceReference; }

            int OutputBufferLength = 0, ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, VoiceReference.Reference, &VoiceReference.TPrompt, null, VoiceReference.Transcript, Text, OutputWavFilename, &OutputBufferLength);

            switch (ErrorCode)
            {
                case 0: { throw new System.Exception("Failed to synthesize pipeline because the pipeline is not initialized."); }
                case -1: { System.Console.WriteLine("[Pipeline Warning]: encode failed, running without reference audio."); } break;
                case -2: { System.Console.WriteLine("[Pipeline Warning]: load_audio failed, running without reference audio."); } break;
                case -3: { throw new System.Exception("[Pipeline Error]: init_kv_cache failed."); }
                case -4: { throw new System.Exception("[Pipeline Error]: generation produced no frames."); }
                case -5: { throw new System.Exception("[Pipeline Error]: decode failed."); }
                case -6: { throw new System.Exception("[Pipeline Error]: save_audio failed."); }
            }

            RawAudioOutput = AudioBuffer.GetRawAudioBuffer();
            return OutputBufferLength;
        }
    }
}
