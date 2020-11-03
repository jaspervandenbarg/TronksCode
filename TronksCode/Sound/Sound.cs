using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// dit is een type waarin verschillende gegevens voor een geluid in opgeslagen kunnen worden.
/// </summary>

[System.Serializable]
public class Sound
{
    public SoundManager.Sounds sound;
    public AudioClip clip;
    [Range(0,1)] public float volume;
    [Range(0, 3)] public float pitch;
    public bool loop;
}
