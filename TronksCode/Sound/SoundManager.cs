using UnityEngine.Audio;
using System;
using UnityEngine;

/// <summary>
/// deze class regelt het activeren van geluid door een nieuwe audiosource te creëeren.
/// </summary>
public static class SoundManager
{
    public enum Sounds
    {
        BulletBounce,
        MineExplosion,
        MinePlacement,
        EntityDeath,
        Music,
    }

    public static void PlaySound(Sounds sound)
    {
        GameObject soundGameObject = new GameObject("SoundSource");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        Sound soundClip = GetAudioClip(sound);
        audioSource.clip = soundClip.clip;
        audioSource.volume = soundClip.volume;
        audioSource.pitch = soundClip.pitch;
        audioSource.loop = soundClip.loop;
        audioSource.outputAudioMixerGroup = sound == Sounds.Music ? SoundAssets.Instance.Music : SoundAssets.Instance.Effects;       
        audioSource.Play();

        if (audioSource.loop)
        {
            MonoBehaviour.DontDestroyOnLoad(soundGameObject);
            return;
        }
        GameObject.Destroy(soundGameObject, soundClip.clip.length);

    }

    public static Sound GetAudioClip(Sounds sound)
    {
        //Debug.Log(sound);
        foreach (Sound soundClip in SoundAssets.Instance.soundClips)
            if (soundClip.sound == sound) return soundClip;     

        Debug.LogError("Sound not found");
        return null;
    }
}
