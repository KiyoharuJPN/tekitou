using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.SetBGMVolume = volume;
    }

    public void SetSEVolume(float volume)
    {
        SoundManager.Instance.SetSEVolume = volume;
    }
}
