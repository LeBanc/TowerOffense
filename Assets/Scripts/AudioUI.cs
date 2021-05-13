using UnityEngine;

/// <summary>
/// AudioUI class defines an AudioClip that will be played when hovering, clicking, selecting or submiting a selectable
/// </summary>
public class AudioUI : MonoBehaviour
{
    public AudioClip audioClip;

    /// <summary>
    /// PlayUISound method plays the UI sound on the AudioManager UI AudioSource
    /// </summary>
    public void PlayUISound()
    {
        if(audioClip != null) AudioManager.PlayOneShotUI(audioClip);
    }    
}
