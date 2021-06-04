using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// SetVolume component links a slider to an AudioMixer volume
/// </summary>
public class SetVolume : MonoBehaviour
{
    public Slider slider;
    public AudioMixer mixer;
    public string targetedVolume = "musicVolume";

    /// <summary>
    /// At Start, link slider value change to volume change and read PlayerPrefs for default value
    /// </summary>
    private void Start()
    {
        slider.onValueChanged.AddListener(SetVolumeLevel);
        slider.value = PlayerPrefs.GetFloat(targetedVolume, 0.8f);        
    }

    /// <summary>
    /// OnDestroy, clear slider listeners
    /// </summary>
    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(SetVolumeLevel);
    }

    /// <summary>
    /// SetVolumeLevel method change the targetedVolume (-80dB;0dB) from a float value (0;100)
    /// </summary>
    /// <param name="_sliderValue">Input value from slider (float)</param>
    public void SetVolumeLevel(float _sliderValue)
    {
        mixer.SetFloat(targetedVolume, Mathf.Max(-40,Mathf.Log10(_sliderValue)) * 20);
        PlayerPrefs.SetFloat(targetedVolume, _sliderValue);
    }
}


