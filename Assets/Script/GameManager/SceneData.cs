
using System;
using Gamepara;

public class SceneData
{
    public readonly static SceneData Instance = new();

    //シーンの名前記録
    public string referer = string.Empty;

    //プレイヤー残機
    public int stock = 2;
    public static int MAX_STOCK = 2;

    //復活時確認用Bool
    public bool revival = false;

    //中間地点起動状態
    public bool wayPoint_1 = false;
    public bool wayPoint_2 = false;

    public void DataReset()
    {
        stock = MAX_STOCK;
        wayPoint_1 = false;
        wayPoint_2 = false;
    }

    //プレイ時間
    public float playTime;
    public float stage1Time;
    public float stage2Time;
    public float stage3Time;

    //プレイ時間記録
    public void PlayTimeSeve(StageType stageType)
    {
        switch (stageType)
        {
            case StageType.stage1:
                stage1Time = playTime;
                break;

            case StageType.stage2:
                stage2Time = playTime;
                break;

            case StageType.stage3:
                stage3Time = playTime;
                break;
        }
    }
}

namespace Gamepara
{
    public enum StageType
    {
        stage1, stage2, stage3
    }

    public struct StagePlayTimes
    {
        public StageType stageType;
        public float stageTime;
    }
}
