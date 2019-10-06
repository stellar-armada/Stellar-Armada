using UnityEngine;

public class MicInput : MonoBehaviour
{

    public static MicInput instance;
        public static float MicLoudness;
        public static float CurrentLoudness;
        private string _device;
     
        public AudioClip recordingMicClip;

        void Awake() => instance = this;
        
        //mic initialization
        public void InitMic(){
            Debug.Log("Mic inited");
            if(_device == null) _device = Microphone.devices[0];
            Debug.Log(Microphone.devices[0]);
            recordingMicClip = Microphone.Start(_device, true, 1, 22050);
        }
     
        void StopMicrophone()
        {
            Microphone.End(_device);
        }
     
 
        int _sampleWindow = 128;
     
        //get data from microphone into audioclip
        float  LevelMax()
        {
            float levelMax = 0;
            float[] waveData = new float[_sampleWindow];
            int micPosition = Microphone.GetPosition(null)-(_sampleWindow+1); // null means the first microphone
            if (micPosition < 0) return 0;
            recordingMicClip.GetData(waveData, micPosition);
            // Getting a peak on the last 128 samples
            for (int i = 0; i < _sampleWindow; i++) {
                float wavePeak = waveData[i] * waveData[i];
                CurrentLoudness = wavePeak;
                if (levelMax < wavePeak) {
                    levelMax = wavePeak;
                }
            }
            return levelMax;
        }
     
     
     
        void Update()
        {
            // levelMax equals to the highest normalized value power 2, a small number because < 1
            // pass the value to a static var so we can access it from anywhere
            MicLoudness = LevelMax ();
        }
     
        bool _isInitialized;
        // start mic when scene starts
        void Start()
        {
            InitMic();
            _isInitialized=true;
        }

        void OnDestroy()
        {
            StopMicrophone();
        }

    }