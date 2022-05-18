using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumSetting : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;

    const string MIXER_MUSIC = "MusicVolum";

    void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolum);
    }

    void SetMusicVolum(float value)
    {
        mixer.SetFloat(MIXER_MUSIC, value+3);

    }
}
