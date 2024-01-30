using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeveData
{
    //�X�e�[�W�̃N���A��Ԃ̃f�[�^
    public EachStageState[] stageState;
    //�c�@��
    public int remain;
}

[System.Serializable]
public class SettingData //���ʐݒ�̃Z�[�u�f�[�^
{
    //BGM
    public float bgmValum;
    //SE
    public float seValum;
}

public class SeveSystem
{
    public readonly static SeveSystem Instance = new();

    /// <summary>
    /// �ݒ�ۑ�
    /// </summary>
    /// <param name="bgmValum"></param>
    /// <param name="seValum"></param>
    public void SettingSeve(float bgmValum, float seValum)
    {
        SettingData settingData = new SettingData()
        {
            bgmValum = bgmValum,
            seValum = seValum
        };

        binarySaveLoad.Save("settingdata", settingData);
    }
    /// <summary>
    /// �ݒ�Ǎ�
    /// </summary>
    /// <returns></returns>
    public SettingData SettingLoad()
    {
        SettingData settingData;

        binarySaveLoad.Load("settingdata", out settingData);

        return settingData;
    }
    /// <summary>
    /// �v���C�f�[�^�ۑ�
    /// </summary>
    /// <param name="stageState"></param>
    /// <param name="remain"></param>
    public void GameDataSeve(EachStageState[] stageState, int remain)
    {
        SeveData seveData = new SeveData()
        {
            stageState = stageState,
            remain = remain
        };

        binarySaveLoad.Save("savedata", seveData);
    }
    /// <summary>
    /// �v���C�f�[�^�Ǎ�
    /// </summary>
    /// <returns></returns>
    public SeveData seveDataLoad()
    {
        SeveData seveData;

        binarySaveLoad.Load("savedata", out seveData);

        return seveData;
    }
    /// <summary>
    /// �v���C�f�[�^�폜
    /// </summary>
    public void seveDataDelete()
    {
        binarySaveLoad.Delete("savedata");
    }
}
