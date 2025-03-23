using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip Clip;
    public AudioSource Source;

    [Range(0f, 1f)] public float Volume;
    [Range(.1f, 3f)] public float Pitch;

    public Sound(AudioSource source)
    {
        Source = source;
    }
}