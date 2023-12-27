using Gamepara;
using System;
using System.Diagnostics;

public class SceneData
{
    public readonly static SceneData Instance = new();

    public struct EachStageState
    {
        public bool openStage;　//ステージが遊べるか
        public bool isClear;  　//クリア確認
        public bool firstOpen;　//開くのが初めてか 
        public float clearTime; //クリアタイム
    }

    //シーンの名前記録
    public string referer = string.Empty;

    private EachStageState[] stageStates = new EachStageState[Enum.GetNames(typeof(StageType)).Length];
    private bool[] stageFirstOpen = new bool[4] { false,false,true,true };

    public EachStageState[] GetEachStageState
    {
        get { return this.stageStates; }
    }

    //プレイヤー残機
    public int stock = 2;
    public static int MAX_STOCK = 2;

    //復活時確認用Bool
    public bool revival = false;

    //中間地点起動状態
    public bool wayPoint_1 = false;
    public bool wayPoint_2 = false;

    public void StageOpen(StageType stageType)
    {
        int stageId = (int)stageType;

        stageStates[stageId].isClear = true;

        if (stageId < stageStates.Length - 1)
        {
            stageId++;
            //次のステージが遊べる状態でなければ変更
            stageStates[stageId].openStage = true;
            if (stageFirstOpen[stageId])
            {
                stageStates[stageId].firstOpen = true;
                stageFirstOpen[stageId] = false;
            }
        }
    }
    public void StagePlay(int stageId)
    {
        stageStates[stageId].firstOpen = false;
    }

    public void StageStateReset()
    {
        stageStates = new EachStageState[Enum.GetNames(typeof(StageType)).Length];
        stageFirstOpen = new bool[4] { false, false, true, true };
    }

    //ステージ状態リセット
    public void StageDataReset()
    {
        stock = MAX_STOCK;
        wayPoint_1 = false;
        wayPoint_2 = false;
    }

    //プレイ時間一時格納用
    public float playTime;

    //プレイ時間取得
    public float[] PlayTimeGet
    {
        get
        {
            float[] clearTimes = new float[]
            {
                stageStates[1].clearTime, stageStates[2].clearTime, stageStates[3].clearTime
            };
            return clearTimes;
        }
    }
    //プレイ時間記録
    public void PlayTimeSeve(StageType stageType)
    {
        switch (stageType)
        {
            case StageType.stage1:
                stageStates[1].clearTime = playTime;
                break;

            case StageType.stage2:
                stageStates[2].clearTime = playTime;
                break;

            case StageType.stage3:
                stageStates[3].clearTime = playTime;
                break;
        }
    }
    //指定したステージのプレイ時間リセット
    public void PlayTimeDelete()
    {
        switch (referer)
        {
            case "stage1":
                stageStates[1].clearTime = 0;
                break;

            case "stage2":
                stageStates[2].clearTime = 0;
                break;

            case "stage3":
                stageStates[3].clearTime = 0;
                break;
        }
    }
    //全ステージのプレイ時間リセット
    public void PlayTimeReset()
    {
        stageStates[1].clearTime = 0;
        stageStates[2].clearTime = 0;
        stageStates[3].clearTime = 0;
    }
}

namespace Gamepara
{
    public enum StageType
    {
        Tutorial,
        stage1, stage2, stage3
    }

    public struct StagePlayTimes
    {
        public StageType stageType;
        public float stageTime;
    }
}
