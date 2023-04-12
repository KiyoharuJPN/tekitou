using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSoundSource : MonoBehaviour
{
    public static EffectSoundSource instance = null;
    AudioSource audioSource = null;

    private void Awake()
    {
        if(instance == null) { instance = this; }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE(AudioClip clip)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            //Debug.Log("�I�[�f�B�I�\�[�X���ݒ肳��Ă��܂���");
        }
    }

    

    public void Volume(float v) { audioSource.volume = v; }
    public float GetVolume() { return audioSource.volume; }

}
