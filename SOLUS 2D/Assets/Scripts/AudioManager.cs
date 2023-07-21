using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundsMixerGroup;

    public Sound[] sounds;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = s.spatialBlend;
            s.source.playOnAwake = s.playOnAwake;

            switch (s.audioType)
            {
                case Sound.AudioTypes.Sounds:
                    s.source.outputAudioMixerGroup = soundsMixerGroup;
                    break;

                case Sound.AudioTypes.Music:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    break;
            }
        }
    }

    public void Play(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        string mixer = "";
        if (s.source.outputAudioMixerGroup == soundsMixerGroup)
        {
            mixer = "Sounds Volume";
        }

        else if (s.source.outputAudioMixerGroup == musicMixerGroup)
        {
            mixer = "Music Volume";
        }
        
        bool value = soundsMixerGroup.audioMixer.GetFloat(mixer, out float volume);
        AudioSource.PlayClipAtPoint(s.source.clip, position, Mathf.Pow(10f, volume / 20));
    }

    public void PlayGlobal(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public void SetVolume(string Mixer, float volume)
    {
        if (Mixer == "Sounds Volume")
        {
            soundsMixerGroup.audioMixer.SetFloat(Mixer, volume);
        }

        else if (Mixer == "Music Volume")
        {
            musicMixerGroup.audioMixer.SetFloat(Mixer, volume);
        }        
    }

}
