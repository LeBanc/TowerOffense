using UnityEngine;

/// <summary>
/// AudioUI class defines an AudioClip that will be played when hovering, clicking, selecting or submiting a selectable
/// </summary>
public class AudioUI : MonoBehaviour
{
    public AudioClip audioBip;
    public AudioClip audioSelection;

    /// <summary>
    /// PlayUIBip method plays the UI bip sound on the AudioManager UI AudioSource
    /// </summary>
    public void PlayUIBip()
    {
        if(audioBip != null) AudioManager.PlayOneShotUI(audioBip, true);
    }

    /// <summary>
    /// PlayUISelection method plays the UI selection sound on the AudioManager UI AudioSource
    /// </summary>
    public void PlayUISelection()
    {
        if (audioSelection != null) AudioManager.PlayOneShotUI(audioSelection);
    }
}
