using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public AudioMixer MasterMixer;

    [SerializeField] private Slider VolumeSlider;
    private float AudioValue;

    private void Start()
    {
        //SetVolume(0);
        if (VolumeSlider == null)
        {
            VolumeSlider = transform.root.GetComponentInChildren<Slider>();
        }

        MasterMixer.GetFloat("MasterVolumeParm",out AudioValue);
        VolumeSlider.value = AudioValue;
    }

    public void SetVolume(float Volume)
    {
        MasterMixer.SetFloat("MasterVolumeParm", Volume);
    }

    public void SetFullscreen(bool IsFullscreen)
    {
        Screen.fullScreen = IsFullscreen;
    }

}
