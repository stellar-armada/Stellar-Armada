using UnityEngine;
using UnityEngine.UI;
using SpaceCommander.Game;

#pragma warning disable 0649
namespace SpaceCommander.Audio
{

    // Added to volume sliders for controlling audio-specific settings

    public class VolumeSliderListener : MonoBehaviour
    {

        enum AudioVolumeType
        {
            Music = 0, Sfx = 1 // TODO: Split announcer into separate mix group and setting
        };

        [SerializeField] AudioVolumeType audioVolumeType;

        Slider slider;

        void Awake()
        {
            slider = GetComponent<Slider>();
        }

        void OnEnable()
        {
            slider.onValueChanged.AddListener(HandleValueChange);

            if (audioVolumeType == AudioVolumeType.Music)
            {
                slider.value = SettingsManager.GetMusicVolume();
            }
            else if (audioVolumeType == AudioVolumeType.Sfx)
            {
                slider.value = SettingsManager.GetSoundFxVolume();

            }
        }

        void OnDisable()
        {
            slider.onValueChanged.RemoveListener(HandleValueChange);

        }

        public void HandleValueChange(float newVal)
        {
            if (slider == null) slider = GetComponent<Slider>();

            if (audioVolumeType == AudioVolumeType.Music)
            {
                MusicController.instance.SetMusicVolume(newVal);

            }
            else if (audioVolumeType == AudioVolumeType.Sfx)
            {
                MusicController.instance.SetSfxVolume(newVal);
            }
        }
    }
}