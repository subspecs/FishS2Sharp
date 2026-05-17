namespace FishS2Sharp
{
    public enum GPUBackendTypes : int
    {
        CPU = -1,

        Vulkan = 0,
        Cuda = 1,
        Metal = 2,
    }

    internal static class Native
    {
        const string DllPath = @"C:\My Storage\My Projects\GIT\s2.cpp_build\RelWithDebInfo\s2.dll";

        internal unsafe delegate int S2StreamingWriteCallback(byte* data, int size, void* user_data);
        internal unsafe delegate void S2StreamingDoneCallback(void* user_data);
        internal unsafe delegate void S2StreamingErrorCallback(sbyte* message, void* user_data);
        internal unsafe delegate int S2StreamingCancelCallback(void* user_data);

        internal unsafe struct S2StreamingCallbacks
        {
            internal S2StreamingWriteCallback on_wav_chunk;
            internal S2StreamingDoneCallback on_done;
            internal S2StreamingErrorCallback on_error;
            internal S2StreamingCancelCallback is_cancelled;
            internal void* user_data;

            //Constructor:
            public S2StreamingCallbacks(S2StreamingWriteCallback on_wav_chunk = null, S2StreamingDoneCallback on_done = null, S2StreamingErrorCallback on_error = null, S2StreamingCancelCallback is_cancelled = null, void* user_data = null)
            {
                this.on_wav_chunk = on_wav_chunk; this.on_done = on_done; this.on_error = on_error; this.is_cancelled = is_cancelled; this.user_data = user_data;
            }
        };

        internal unsafe struct S2StreamingParams
        {
            internal int stream_decode_stride_frames;
            internal int stream_holdback_frames;
            internal int codec_decode_context_frames;
            internal int low_latency;
            internal int segment_sentences;
            internal int sentence_pause_ms;
            internal int segment_max_chars;
            internal sbyte* voice;
            internal sbyte* voice_dir;

            //Constructor:
            public S2StreamingParams(int stream_decode_stride_frames = 0, int stream_holdback_frames = -1, int codec_decode_context_frames = -1, int low_latency = 0, int segment_sentences = 0, int sentence_pause_ms = 180, int segment_max_chars = 0, sbyte* voice = null, sbyte* voice_dir = null)
            {
                this.stream_decode_stride_frames = stream_decode_stride_frames; this.stream_holdback_frames = stream_holdback_frames; this.codec_decode_context_frames = codec_decode_context_frames; 
                this.low_latency = low_latency; this.segment_sentences = segment_sentences; this.sentence_pause_ms = sentence_pause_ms; this.segment_max_chars = segment_max_chars; this.voice = voice; this.voice_dir = voice_dir;
            }
        };

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void* AllocS2Pipeline();
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void ReleaseS2Pipeline(void* Pipeline);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void SetS2LogLevel(int LogLevel);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int GetS2LogLevel();

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern void SyncS2TokenizerConfigFromS2Model(void* Model, void* Tokenizer);

        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2Pipeline(void* Pipeline, void* Tokenizer, void* Model, void* AudioCodec);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2PipelineFromFiles(void* Pipeline, string gguf_path, string tokenizer_path, int gpu_device, GPUBackendTypes backend_type, int n_gpu_layers, int codec_follow_backend);

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
        public static unsafe extern int InitializeS2Model(void* Model, string gguf_path, int gpu_device, GPUBackendTypes backend_type);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2ModelWithGpuLayers(void* Model, string gguf_path, int gpu_device, int backend_type, int n_gpu_layers);


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
        public static unsafe extern int InitializeS2AudioCodec(void* AudioCodec, string gguf_path, int gpu_device, GPUBackendTypes backend_type);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int InitializeS2AudioCodecModelShared(void* Model, void* AudioCodec, string gguf_path, int gpu_device, GPUBackendTypes backend_type);

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
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int S2SynthesizeStreaming(void* Pipeline, void* GenerateParams, S2StreamingCallbacks* StreamingCallbacks, void* ReferenceAudioPromptCodes, int* ReferenceAudioTPrompt, string ReferenceAudioPath, string ReferenceAudioTranscript, string TextToInfer, int StreamDecodeStrideFrames);
        [System.Runtime.InteropServices.DllImport(DllPath)]
        public static unsafe extern int S2SynthesizeStreaming(void* Pipeline, void* GenerateParams, S2StreamingCallbacks* StreamingCallbacks, void* ReferenceAudioPromptCodes, int* ReferenceAudioTPrompt, string ReferenceAudioPath, string ReferenceAudioTranscript, string TextToInfer, S2StreamingParams* StreamingParams);
    }

    public class FishModel
    {
        public int DeviceID { get { return _DeviceID; } }
        public GPUBackendTypes Backend { get { return _BackendType; } }
        public string ModelPath { get { return _ModelPath; } }
        public string TokenizerPath { get { return _TokenizerPath; } }

        private int _DeviceID;
        private bool IsDisposed;
        internal string _ModelPath, _TokenizerPath;
        internal unsafe void* ModelHandle, TokenizerHandle, AudioCodecHandle, DummyPipeline;
        private GPUBackendTypes _BackendType;

        //Constructors:
        public FishModel(string ModelPath, string TokenizerPath, GPUBackendTypes BackendType, int DeviceID = 0)
        {
            _ModelPath = ModelPath; _TokenizerPath = TokenizerPath; _BackendType = BackendType; _DeviceID = ((int)BackendType < 0 ? -1 : DeviceID);

            unsafe
            {
                ModelHandle = Native.AllocS2Model();
                AudioCodecHandle = Native.AllocS2AudioCodec();

                if (Native.InitializeS2AudioCodecModelShared(ModelHandle, AudioCodecHandle, ModelPath, _DeviceID, BackendType) != 1)
                {
                    throw new System.Exception("Failed to initialize model/audio codec with " + BackendType.ToString() + " support.");
                }

                TokenizerHandle = Native.AllocS2Tokenizer();
                if (Native.InitializeS2Tokenizer(TokenizerHandle, _TokenizerPath) != 1)
                {
                    throw new System.Exception("Failed to initialize tokenizer with path '" + _TokenizerPath + "'.");
                }
                Native.SyncS2TokenizerConfigFromS2Model(ModelHandle, TokenizerHandle);

                DummyPipeline = Native.AllocS2Pipeline();
                if (Native.InitializeS2Pipeline(DummyPipeline, TokenizerHandle, ModelHandle, AudioCodecHandle) != 1)
                {
                    throw new System.Exception("Failed to initialize pipeline.");
                }
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
                unsafe
                {
                    Native.ReleaseS2Pipeline(DummyPipeline);
                    Native.ReleaseS2Tokenizer(TokenizerHandle);
                    Native.ReleaseS2Model(ModelHandle);
                    Native.ReleaseS2AudioCodec(AudioCodecHandle);
                }
                IsDisposed = true;
            }
        }
    }
    public class FishAudioParameters
    {
        public int MaxTokens { get { return _max_new_tokens; } set { _max_new_tokens = value; OnParamUpate(); } }
        public float Temp { get { return _temperature; } set { _temperature = value; OnParamUpate(); } }
        public float Top_P { get { return _top_p; } set { _top_p = value; OnParamUpate(); } }
        public int Top_K { get { return _top_k; } set { _top_k = value; OnParamUpate(); } }
        public int MinTokensBeforeEnd { get { return _min_tokens_before_end; } set { _min_tokens_before_end = value; OnParamUpate(); } }
        public int ThreadCount { get { return _n_threads; } set { _n_threads = value; OnParamUpate(); } }
        public bool Verbose { get { return _verbose; } set { _verbose = value; OnParamUpate(); } }

        private float _temperature, _top_p;
        private int _max_new_tokens, _top_k, _min_tokens_before_end, _n_threads;
        private bool _verbose;

        internal unsafe void* GenerateParams;
        private bool IsDisposed;

        //Constructor:
        public FishAudioParameters()
        {
            unsafe
            {
                GenerateParams = Native.AllocS2GenerateParams();

                _max_new_tokens = 1024;
                _temperature = 0.8f;
                _top_p = 0.8f;
                _top_k = 30;
                _min_tokens_before_end = 0;
                _n_threads = 0;
                _verbose = true;
            }
            OnParamUpate();
        }
        ~FishAudioParameters()
        {
            Dispose();
        }

        //Internal Methods:
        private void OnParamUpate()
        {
            if (IsDisposed) { throw new System.ObjectDisposedException("FishAudioParameters"); }
            unsafe
            {
                Native.InitializeS2GenerateParams(GenerateParams, _max_new_tokens, _temperature, _top_p, _top_k, _min_tokens_before_end, _n_threads, _verbose ? 1 : 0);
            }
        }

        //Public Methods:
        public void Dispose()
        {
            if (!IsDisposed)
            {
                unsafe
                {
                    Native.ReleaseS2GenerateParams(GenerateParams);
                }
                IsDisposed = true;
            }
        }
    }
    public class FishAudioVoiceReference
    {
        public string Name { get { return _Name; } }
        public string VoiceTranscript { get { return _VoiceTranscript; } }
        public FishModel BindedModel { get { return _Model; } }

        internal int TPrompt;
        private FishModel _Model;
        internal unsafe void* Reference;
        private string _Name, _VoiceTranscript;

        //Constructor
        public FishAudioVoiceReference(FishModel Model, string Name, string Mp3WavFileName, string VoiceTranscript)
        {
            this._Name = Name; this._VoiceTranscript = VoiceTranscript; _Model = Model;

            unsafe
            {
                Reference = Native.AllocS2AudioPromptCodes(); TPrompt = 0;
                fixed (int* _TPrompt = &TPrompt)
                {
                    if (Native.InitializeAudioPromptCodes(Model.DummyPipeline, System.Environment.ProcessorCount, Mp3WavFileName, Reference, _TPrompt) != 1)
                    {
                        throw new System.Exception("Failed to create reference voice.");
                    }
                }
            }
        }
        ~FishAudioVoiceReference()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose()
        {
            unsafe
            {
                Native.ReleaseS2AudioPromptCodes(Reference);
            }
        }
    }

    public class FishS2Client
    {
        private class FishAudioBuffer
        {
            public int SampleCount { get { return _SampleCount; } }
            public int BufferCapacity { get { return _BufferCapacity; } }
            public unsafe float* AudioSampleHandle { get { return Native.GetS2AudioBufferDataPointer(AudioBufferHandle); } }

            internal int _SampleCount, _BufferCapacity;
            internal unsafe void* AudioBufferHandle;
            private bool IsDisposed;

            //Constructors:
            internal FishAudioBuffer()
            {
                unsafe
                {
                    AudioBufferHandle = Native.AllocS2AudioBuffer();
                }
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
                    unsafe
                    {
                        Native.ReleaseS2AudioBuffer(AudioBufferHandle);
                    }
                    IsDisposed = true;
                }
            }
        }

        public FishModel Model { get { return _Model; } }

        private FishModel _Model;
        private FishAudioBuffer AudioBuffer;

        private unsafe void* Pipeline;
        private bool IsDisposed;

        //Constructors:
        public FishS2Client(FishModel Model)
        {
            _Model = Model; AudioBuffer = new FishAudioBuffer();
            unsafe
            {
                Pipeline = Native.AllocS2Pipeline();
                if (Native.InitializeS2Pipeline(Pipeline, _Model.TokenizerHandle, _Model.ModelHandle, _Model.AudioCodecHandle) != 1)
                {
                    throw new System.Exception("Failed to initialize pipeline.");
                }
            }
        }
        public FishS2Client(string ModelPath, string TokenizerPath, GPUBackendTypes BackendType, int DeviceID = 0)
        {
            _Model = new FishModel(ModelPath, TokenizerPath, BackendType, DeviceID); AudioBuffer = new FishAudioBuffer();
            unsafe
            {
                Pipeline = Native.AllocS2Pipeline();
                if (Native.InitializeS2Pipeline(Pipeline, _Model.TokenizerHandle, _Model.ModelHandle, _Model.AudioCodecHandle) != 1)
                {
                    throw new System.Exception("Failed to initialize pipeline.");
                }
            }
        }
        ~FishS2Client()
        {
            Dispose();
        }

        //Public Methods:
        public void Dispose(bool DisposeModel = true)
        {
            if (!IsDisposed)
            {
                AudioBuffer.Dispose();
                unsafe
                {
                    Native.ReleaseS2Pipeline(Pipeline);
                }
                if (DisposeModel) { _Model.Dispose(); }
                IsDisposed = true;
            }
        }

        public void Synthesize(string Text, string OutputWavFilename, FishAudioParameters Parameters, FishAudioVoiceReference Voice = null)
        {
            if (IsDisposed) { throw new System.ObjectDisposedException("FishS2Client"); }

            int OutputBufferLength = 0, ErrorCode = -1;
            unsafe
            {
                if (Voice != null)
                {
                    fixed (int* _TPrompt = &Voice.TPrompt)
                    {
                        ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, Voice.Reference, _TPrompt, null, Voice.VoiceTranscript, Text, OutputWavFilename, &OutputBufferLength);
                    }
                }
                else { ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, null, null, null, null, Text, OutputWavFilename, &OutputBufferLength); }
            }

            switch (ErrorCode)
            {
                case 0: { throw new System.Exception("Failed to synthesize pipeline because the pipeline is not initialized."); }
                case -1: { System.Console.WriteLine("[Pipeline Warning]: encode failed, running without reference audio."); } break;
                case -2: { System.Console.WriteLine("[Pipeline Warning]: load_audio failed, running without reference audio."); } break;
                case -3: { throw new System.Exception("[Pipeline Error]: init_kv_cache failed."); }
                case -4: { throw new System.Exception("[Pipeline Error]: generation produced no frames."); }
                case -5: { throw new System.Exception("[Pipeline Error]: decode failed."); }
                case -6: { throw new System.Exception("[Pipeline Error]: save_audio failed."); }
                case -7: { throw new System.Exception("[Pipeline Error]: reference voice is missing transcript text."); }
                case -8: { throw new System.Exception("[Pipeline Error]: reference voice TPrompt value is 0."); }
            }

            AudioBuffer._BufferCapacity = AudioBuffer._BufferCapacity < OutputBufferLength ? OutputBufferLength : AudioBuffer._BufferCapacity;
        }
        public unsafe int Synthesize(string Text, float* RawAudioOutput, FishAudioParameters Parameters, FishAudioVoiceReference Voice = null)
        {
            if (IsDisposed) { throw new System.ObjectDisposedException("FishS2Client"); }

            int OutputBufferLength = 0, ErrorCode = -1;
            unsafe
            {
                if (Voice != null)
                {
                    fixed (int* _TPrompt = &Voice.TPrompt)
                    {
                        ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, Voice.Reference, _TPrompt, null, Voice.VoiceTranscript, Text, null, &OutputBufferLength);
                    }
                }
                else { ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, null, null, null, null, Text, null, &OutputBufferLength); }
            }

            switch (ErrorCode)
            {
                case 0: { throw new System.Exception("Failed to synthesize pipeline because the pipeline is not initialized."); }
                case -1: { System.Console.WriteLine("[Pipeline Warning]: encode failed, running without reference audio."); } break;
                case -2: { System.Console.WriteLine("[Pipeline Warning]: load_audio failed, running without reference audio."); } break;
                case -3: { throw new System.Exception("[Pipeline Error]: init_kv_cache failed."); }
                case -4: { throw new System.Exception("[Pipeline Error]: generation produced no frames."); }
                case -5: { throw new System.Exception("[Pipeline Error]: decode failed."); }
                case -6: { throw new System.Exception("[Pipeline Error]: save_audio failed."); }
                case -7: { throw new System.Exception("[Pipeline Error]: reference voice is missing transcript text."); }
                case -8: { throw new System.Exception("[Pipeline Error]: reference voice TPrompt value is 0."); }
            }

            RawAudioOutput = AudioBuffer.AudioSampleHandle;
            AudioBuffer._BufferCapacity = AudioBuffer._BufferCapacity < OutputBufferLength ? OutputBufferLength : AudioBuffer._BufferCapacity;
            return OutputBufferLength;
        }
        public unsafe int Synthesize(string Text, string OutputWavFilename, float* RawAudioOutput, FishAudioParameters Parameters, FishAudioVoiceReference Voice = null)
        {
            if (IsDisposed) { throw new System.ObjectDisposedException("FishS2Client"); }

            int OutputBufferLength = 0, ErrorCode = -1;
            unsafe
            {
                if (Voice != null)
                {
                    fixed (int* _TPrompt = &Voice.TPrompt)
                    {
                        ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, Voice.Reference, _TPrompt, null, Voice.VoiceTranscript, Text, OutputWavFilename, &OutputBufferLength);
                    }
                }
                else { ErrorCode = Native.S2Synthesize(Pipeline, Parameters.GenerateParams, AudioBuffer.AudioBufferHandle, null, null, null, null, Text, OutputWavFilename, &OutputBufferLength); }
            }

            switch (ErrorCode)
            {
                case 0: { throw new System.Exception("Failed to synthesize pipeline because the pipeline is not initialized."); }
                case -1: { System.Console.WriteLine("[Pipeline Warning]: encode failed, running without reference audio."); } break;
                case -2: { System.Console.WriteLine("[Pipeline Warning]: load_audio failed, running without reference audio."); } break;
                case -3: { throw new System.Exception("[Pipeline Error]: init_kv_cache failed."); }
                case -4: { throw new System.Exception("[Pipeline Error]: generation produced no frames."); }
                case -5: { throw new System.Exception("[Pipeline Error]: decode failed."); }
                case -6: { throw new System.Exception("[Pipeline Error]: save_audio failed."); }
                case -7: { throw new System.Exception("[Pipeline Error]: reference voice is missing transcript text."); }
                case -8: { throw new System.Exception("[Pipeline Error]: reference voice TPrompt value is 0."); }
            }

            RawAudioOutput = AudioBuffer.AudioSampleHandle;
            AudioBuffer._BufferCapacity = AudioBuffer._BufferCapacity < OutputBufferLength ? OutputBufferLength : AudioBuffer._BufferCapacity;
            return OutputBufferLength;
        }
    }
}
