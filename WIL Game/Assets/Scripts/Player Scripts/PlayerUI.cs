using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerUI : MonoBehaviour
{

    public AudioMixer MasterMixer;

    private void Start()
    {
        SetVolume(0);
    }

    public void SetVolume(float SetVolume)
    {
        MasterMixer.SetFloat("MasterVolumeParm", SetVolume);
    }

    public void SetFullscreen(bool IsFullscreen)
    {
        Screen.fullScreen = IsFullscreen;
    }

}
