using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTestScript : MonoBehaviour
{
    public AudioClip musicClip;
    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        musicClip = (AudioClip)Resources.Load("Effects/Laser3");
        audioSource = GameObject.FindGameObjectWithTag("AudioSource").GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.volume = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            audioSource.Play();
    }
}
