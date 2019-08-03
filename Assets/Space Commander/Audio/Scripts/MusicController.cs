using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using SpaceCommander.Game;

namespace SpaceCommander.Audio
{

    // Script to control 

    public class MusicController : MonoBehaviour
    {

        public static MusicController instance; // Singleton accessor


        #region PrivateFieldsSetByInspector
        [SerializeField] AudioMixer audioMixer;

        [SerializeField] AudioMixerGroup inGameMusicMixerGroup;

        [SerializeField] private MusicData musicData;

        [SerializeField] AudioSource titleMusicAudioSource;

        [SerializeField] bool playTitleMusicAtStart = true;

        [SerializeField] float musicCrossFadeTime = 2f;
        #endregion

        #region PrivateFields

        bool musicShouldBePlaying = false;

        int currentTrack = 0;

        public static AudioSource[] aud = new AudioSource[2];   //create an array with 2 audio sources that we swap between for transitions

        bool activeMusicSource; //boolean to determine which audio source is the current one
        
        IEnumerator musicTransition; //We will store the transition as a Coroutine so that we have the ability to stop it halfway if necessary

        #endregion

        #region Initialization and Deinitialization
        public void Init()
        {
            instance = this;
        }
        void Awake() // Calls when object is created
        {
            //Create the AudioSource components that we will be using
            aud[0] = gameObject.AddComponent<AudioSource>();
            aud[1] = gameObject.AddComponent<AudioSource>();


            SetMusicVolume(SettingsManager.GetMusicVolume());
            SetSfxVolume(SettingsManager.GetSoundFxVolume());
        }
        private void Start() // calls when object is first activated
        {
            if (playTitleMusicAtStart)
            {
                Invoke(nameof(StartTitleMusic), 1f); // invoke after start so it doesn't skip while things are still loading

            }
        }

        private void OnDestroy()
        {
            instance = null; // Unity doesn't reinit static vars on scene load, so make sure you null them
        }
        #endregion

        #region PublicMethods

        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("MusicVolume", value);
            SettingsManager.SetMusicVolume(value);

        }
        public void SetSfxVolume(float value)
        {

            audioMixer.SetFloat("SfxVolume", value);

            SettingsManager.SetSoundFxVolume(value);
        }
        public void StartTitleMusic()
        {
            titleMusicAudioSource.clip = musicData.titleTrack;
            titleMusicAudioSource.Play();
        }

        public void StopTitleMusic()
        {
            instance.StartCoroutine(FadeOutTitleMusic(titleMusicAudioSource));

        }
        public void StartInGameMusic()
        {
            musicShouldBePlaying = true;

        }
        #endregion

        #region PrivateMethods

        IEnumerator FadeOutTitleMusic(AudioSource audio)
        {
            float volume = 1f;

            while (volume < musicCrossFadeTime)
            {
                volume += Time.deltaTime;
                audio.volume = Mathf.Lerp(1f, 0, volume / musicCrossFadeTime);
                yield return null;
            }
            audio.Stop();
        }

        void PlayNextSong()
        {
            currentTrack = Random.Range(0, musicData.gameTracks.Length);
            NewSoundtrack(musicData.gameTracks[currentTrack]);
        }

        // Update is called once per frame
        void Update()
        {
            if (musicShouldBePlaying && !aud[0].isPlaying && !aud[1].isPlaying)
            {


                PlayNextSong();
            }
        }

        void NewSoundtrack(AudioClip clip)
        {
            int nextSource = !activeMusicSource ? 0 : 1;
            int currentSource = activeMusicSource ? 0 : 1;

            aud[currentSource].outputAudioMixerGroup = inGameMusicMixerGroup;
            aud[nextSource].outputAudioMixerGroup = inGameMusicMixerGroup;

            //If the clip is already being played on the current audio source, we will end now and prevent the transition
            if (clip == aud[currentSource].clip)
                return;

            //If a transition is already happening, we stop it here to prevent our new Coroutine from competing
            if (musicTransition != null)
                StopCoroutine(musicTransition);

            aud[nextSource].clip = clip;
            aud[nextSource].Play();

            musicTransition = Transition(20); //20 is the equivalent to 2 seconds (More than 3 seconds begins to overlap for a bit too long)
            StartCoroutine(musicTransition);
        }

        IEnumerator Transition(int transitionDuration) //  'transitionDuration' is how many tenths of a second it will take, eg, 10 would be equal to 1 second
        {

            for (int i = 0; i < transitionDuration + 1; i++)
            {
                aud[0].volume = activeMusicSource ? (transitionDuration - i) * (1f / transitionDuration) : (0 + i) * (1f / transitionDuration);
                aud[1].volume = !activeMusicSource ? (transitionDuration - i) * (1f / transitionDuration) : (0 + i) * (1f / transitionDuration);

                yield return new WaitForSecondsRealtime(0.1f);
                //use realtime otherwise if you pause the game you could pause the transition half way
            }

            //finish by stopping the audio clip on the now silent audio source
            aud[activeMusicSource ? 0 : 1].Stop();

            activeMusicSource = !activeMusicSource;
            musicTransition = null;
        }

        #endregion



    }
}