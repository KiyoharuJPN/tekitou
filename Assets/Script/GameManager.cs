using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("�V�[��ID�i0:�^�C�g��,1:�`���[�g���A��,2:�X�e�[�W1,3:���U���g,4:�Q�[���I�[�o�[")]
    [Range(0, 4)]
    [SerializeField]
    int ID;

    void Start()
    {
        switch (ID)
        {
            case 0:
                BGMStart_Title();
                break;
            case 1:
                BGMStart_Tutorial();
                break;
            case 2:
                BGMStart_Stage1();
                break;
            case 3:
                BGMStart_Result();
                break;
            case 4:
                BGMStart_GameOver();
                break;
        }
        
    }

    private void BGMStart_Title()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title, BGMSoundData.BGM.none);
    }
    private void BGMStart_Tutorial()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Tutorial_intro, BGMSoundData.BGM.Tutorial_roop);
    }
    private void BGMStart_Stage1()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Stage1_intro, BGMSoundData.BGM.Stage1_roop);
    }
    private void BGMStart_Result()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Result, BGMSoundData.BGM.none);
    }
    private void BGMStart_GameOver()
    {
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.GameOver, BGMSoundData.BGM.none);
    }

}
