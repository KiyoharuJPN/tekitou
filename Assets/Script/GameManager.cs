using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BGMStart());
    }

    IEnumerator BGMStart()
    {
        
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage1_intro);

        while (SoundManager.Instance.BGMEnd())
        {
            yield return null;
        }

        SoundManager.Instance.BGMLoopSwich();
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage1_roop);
    }

}
