using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource Emitter;
    public bool SoundPlaying = false;
    public enum SoundOptions
    {
        Moving,
        TakeDamage,
        Attack,
        Death,
        Silence
    };

    public SoundOptions CurrentSound;


    [SerializeField] private Sound[] EnemySounds;

    private void Start()
    {
        Emitter = GetComponent<AudioSource>();
    }

    private void Update()
    {
        SoundPlaying = Emitter.isPlaying;
    }

    public void PlaySound(SoundOptions SelectedSound)
    {
        //if (CurrentSound==SelectedSound) { Debug.Log("Sounder"); return; }
        StopSound();
        Debug.Log("Sounder      "+SelectedSound);
        bool Exists = EnemySounds.Any(ES => ES.Name == SelectedSound.ToString());
        if (!Exists) { Debug.LogError("Sound no existo"); return; }
        Sound SoundRef = EnemySounds.FirstOrDefault(Snd => Snd.Name == SelectedSound.ToString());

        Emitter.clip = SoundRef.SoundClip;
        Emitter.volume = SoundRef.Volume;
        //Emitter.pitch = SoundRef.Pitch;
        Emitter.loop = SoundRef.Looping;
        CurrentSound = SelectedSound;
        Emitter.Play();
    }

    public void StopSound()
    {
        Emitter.Stop();
    }

}
