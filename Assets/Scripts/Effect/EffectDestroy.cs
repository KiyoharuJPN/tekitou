using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
    public SESoundData.SE playSe;
    void ThisDestroy()
    {
        Destroy(gameObject);
    }
    void SoundEffect()
    {
        SoundManager.Instance.PlaySE(playSe);
    }
}
