using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if(!_audioSource){
            Debug.LogError("Audio Source is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio(AudioClip _audioClip){
        _audioSource.clip = _audioClip;
        _audioSource.Play();
    }
}
