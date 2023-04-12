using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToSEVolume : MonoBehaviour
{
    public AudioSource AS;

    private void Update()
    {
        if(AS.volume != EffectSoundSource.instance.GetVolume())
        {
            AS.volume = EffectSoundSource.instance.GetVolume();
        }
    }
}
