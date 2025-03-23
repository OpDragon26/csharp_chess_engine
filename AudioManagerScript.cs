using Unity.VisualScripting;
using UnityEngine;

// Sounds from https://pixabay.com
public class AudioManagerScript : MonoBehaviour
{
    public Sound[] Sounds = new Sound[4];
    public 

    void Awake()
    {
        foreach (Sound sound in Sounds)
        {
            sound.Source = gameObject.AddComponent<AudioSource>();
            
            sound.Source.clip = sound.Clip;
            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
        }
    }

    public void PlaySound(Audios sound)
    {
        Sounds[(int)sound].Source.Play();
    }
}

public enum Audios
{
    move,
    capture,
    check,
    click
}

