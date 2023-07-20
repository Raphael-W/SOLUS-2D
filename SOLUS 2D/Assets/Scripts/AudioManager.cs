using UnityEngine.Audio;
using System;
using UnityEngine;
using Unity.Netcode;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    [ClientRpc]
    public void PlayClientRpc(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        AudioSource.PlayClipAtPoint(s.source.clip, position, 1);
    }

}
