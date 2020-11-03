using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// dit script slaat alle audio files en ports op die gebruikt worden
/// </summary>
public class SoundAssets : MonoBehaviour
{
    public static SoundAssets Instance { get; private set; }
    public AudioMixerGroup Master;
    public AudioMixerGroup Music;
    public AudioMixerGroup Effects;

    private void Awake()
    {
        Instance = this;
    }

    public Sound[] soundClips;   
}

[System.Serializable]
public class SoundClip
{
    public SoundManager.Sounds sound;
    public AudioClip audioClip;
    [Range(0, 1)] public float volume;
}

