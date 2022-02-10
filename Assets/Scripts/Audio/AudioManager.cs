using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        
    }

    private void Start()
    {
        Play("theme");
    }

    public void Play(string _soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _soundName);
        if (s == null)
        {
            return;
        }
        s.source.Play();
    }

    public void Stop(string _soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == _soundName);
        if (s == null)
        {
            return;
        }
        s.source.Stop();
    }
}
