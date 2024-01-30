using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SeveData
{
    //ステージのクリア状態のデータ
    public EachStageState[] stageState;
    //残機数
    public int remain;
}

[System.Serializable]
public class SettingData //音量設定のセーブデータ
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
    /// 設定保存
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
    /// 設定読込
    /// </summary>
    /// <returns></returns>
    public SettingData SettingLoad()
    {
        SettingData settingData;

        binarySaveLoad.Load("settingdata", out settingData);

        return settingData;
    }
    /// <summary>
    /// プレイデータ保存
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
    /// プレイデータ読込
    /// </summary>
    /// <returns></returns>
    public SeveData seveDataLoad()
    {
        SeveData seveData;

        binarySaveLoad.Load("savedata", out seveData);

        return seveData;
    }
    /// <summary>
    /// プレイデータ削除
    /// </summary>
    public void seveDataDelete()
    {
        binarySaveLoad.Delete("savedata");
    }
}
