using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    private AudioSource _musicAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        _musicAudioSource = GetComponent<AudioSource>();
        if (_musicAudioSource == null) Debug.LogError("[AudioManager] Music AudioSource can't be found");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
