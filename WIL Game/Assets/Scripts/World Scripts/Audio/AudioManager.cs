using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    private void Awake()
    {
        foreach (Sound CurrentSound in Sounds)
        {
            CurrentSound.Source = gameObject.AddComponent<AudioSource>();
            CurrentSound.Source.clip = CurrentSound.SoundClip;

            CurrentSound.Source.volume = CurrentSound.Volume;
            CurrentSound.Source.pitch = CurrentSound.Pitch;
            CurrentSound.Source.loop = CurrentSound.Looping;
        }
    }

}
