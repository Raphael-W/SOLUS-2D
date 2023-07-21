using UnityEngine;

[System.Serializable]
public class Sound
{
    public enum AudioTypes { Music, Sounds}
    public AudioTypes audioType;

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    [Range(0f, 1f)]
    public float spatialBlend;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;
}

