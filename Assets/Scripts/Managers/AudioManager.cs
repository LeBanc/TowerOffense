using System.Collections;
using UnityEngine;

/// <summary>
/// AudioManager class manages the Audio/Sound of the game
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    // AudioSources for music and UI sounds
    public AudioSource musicAudioSource;
    public AudioSource uiAudioSource;

    // AudioClips for musics
    public AudioClip attackMusic;
    public AudioClip idleMusic;

    // fade time when changing music
    public float musicChangeDelay = 0.5f;

    // private AudioListener for StartMenu (no Camera)
    private AudioListener defaultAudioListener;

    /// <summary>
    /// At Start, fetch the AudioListener, launch attack music (on Start menu) and subscribe to events
    /// </summary>
    private void Start()
    {
        defaultAudioListener = GetComponent<AudioListener>();
        PlayAttackMusic();
        PlayManager.OnLoadSquadsOnNewDay += PlayAttackMusic; // Attack music when starting a new day
        PlayManager.OnHQPhase += PlayIdleMusic; // Idle music on HQ phase
        GameManager.OnPauseToLoad += StopMusic; // Stopping music when loading
        GameManager.OnStartToLoad += StopMusic; // Stopping music when loading
    }

    /// <summary>
    /// OnDestroy unsubscribes from events
    /// </summary>
    protected override void OnDestroy()
    {
        PlayManager.OnLoadSquadsOnNewDay -= PlayAttackMusic;
        PlayManager.OnHQPhase -= PlayIdleMusic;
        GameManager.OnPauseToLoad -= StopMusic;
        GameManager.OnStartToLoad -= StopMusic;
        base.OnDestroy();
    }

    /// <summary>
    /// PlayOnShotUI static method plays one sound on the UI AudioSource
    /// </summary>
    /// <param name="_clip">Sound to be played (AudioClip)</param>
    /// <param name="_override">True to override current sound, false otherwise - default is false (bool)</param>
    public static void PlayOneShotUI(AudioClip _clip, bool _override = false)
    {
        if(Instance.uiAudioSource != null)
        {
            if(_override || !Instance.uiAudioSource.isPlaying) Instance.uiAudioSource.PlayOneShot(_clip);
        }
    }

    /// <summary>
    /// SetActiveDefaultAudioListener method enables or disables the default AudioListener
    /// </summary>
    /// <param name="_active">True for activation, false otherwise (bool)</param>
    public static void SetActiveDefaultAudioListener(bool _active)
    {
        if(Instance != null) Instance.defaultAudioListener.enabled = _active;
    }

    /// <summary>
    /// PlayAttackMusic method changes the music to the attack music
    /// </summary>
    void PlayAttackMusic()
    {
        if(attackMusic != null)
        {
            StartCoroutine(ChangeMusic(attackMusic));
        }
    }

    /// <summary>
    /// PlayIdleMusic method changes the music to the idle music
    /// </summary>
    void PlayIdleMusic()
    {
        if (idleMusic != null)
        {
            StartCoroutine(ChangeMusic(idleMusic));
        }
    }

    /// <summary>
    /// StopMusic method stops the music
    /// </summary>
    void StopMusic()
    {
        musicAudioSource.Stop();
    }

    /// <summary>
    /// ChangeMusic coroutine fades out from the current music, change the music to the AudioClip in parameters then fades in
    /// </summary>
    /// <param name="_newClip">Music to be played (AudioClip)</param>
    /// <returns></returns>
    IEnumerator ChangeMusic(AudioClip _newClip)
    {
        float _volume = musicAudioSource.volume;

        // Fade out
        float _t = 0f;
        while(_t < musicChangeDelay)
        {
            musicAudioSource.volume = Mathf.Lerp(_volume, 0f, _t);
            _t += Time.deltaTime;
            yield return null;
        }
        musicAudioSource.volume = 0f;

        // Music change (and play id music is stopped)
        musicAudioSource.clip = _newClip;
        if (!musicAudioSource.isPlaying) musicAudioSource.Play();

        // Fade in
        _t = 0f;
        while (_t < musicChangeDelay)
        {
            musicAudioSource.volume = Mathf.Lerp(0f, _volume, _t);
            _t += Time.deltaTime;
            yield return null;
        }
        musicAudioSource.volume = _volume;
    }
}
