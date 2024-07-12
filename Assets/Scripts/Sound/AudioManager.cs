using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private bool musicIsPlaying = false;
    public AK.Wwise.Event sceneMusic;
   
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.reverbZoneMix = s.reverbZoneMix;
            s.source.loop = s.loop;
        }
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name == name);
        s.source.Stop();
    }

    public void playSound(AK.Wwise.Event playEvent, GameObject childObject)
    {
        playEvent.Post(childObject);
    }
    public void playSoundName(string name, GameObject childObject)
    {
        AkSoundEngine.PostEvent(name, childObject);
    }

    public void toggleMusic()
    {
        if (musicIsPlaying)
        {
            musicIsPlaying = false;
            sceneMusic.Stop(gameObject);
        }
        else
        {
            musicIsPlaying = true;
            Debug.Log("PLAYING MUSIC");
            sceneMusic.Post(gameObject);
        }


    }
}
