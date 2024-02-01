using Gamepara;
using System;
using System.Diagnostics;
using Unity.VisualScripting;

[System.Serializable]
public struct EachStageState
{
    public bool openStage; //ステージが遊べるか
    public bool isClear;   //クリア確認
    public bool firstOpen; //開くのが初めてか 
    public float clearTime; //クリアタイム
    public bool newClearTime; //クリアタイムを更新したか
    public ClearRank clearRank; //クリアランク
}

public class SceneData
{
    public readonly static SceneData Instance = new();

    //シーンの名前記録
    public string referer = string.Empty;

    private EachStageState[] stageStates = new EachStageState[Enum.GetNames(typeof(StageType)).Length];
    private bool[] stageFirstOpen = new bool[4] { false,false,true,true };

    public EachStageState[] GetEachStageState
    {
        get { return this.stageStates; }
        set { this.stageStates = value; }
    }

    //音量
    private float bgmVolume;
    private float seVolume;

    public float getBGMVolume => bgmVolume;
    public float getSEVolume => seVolume;

    public void SetVolume(float bgmVolume, float seVolume)
    {
        this.bgmVolume = bgmVolume;
        this.seVolume = seVolume;
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
    public bool[] NewPlayTimeBoolGet
    {
        get
        {
            bool[] clearTimes = new bool[]
            {
                stageStates[1].newClearTime, stageStates[2].newClearTime, stageStates[3].newClearTime
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

    //現在のセーブデータにてプレイタイムを更新したか
    public bool NewPlayTimeCheck(int stageId, float playTime)
    {
        if (stageStates[stageId].clearTime == 0) {
            stageStates[stageId].newClearTime = true;
            return true; 
        }

        if(stageStates[stageId].clearTime > playTime)
        {
            stageStates[stageId].newClearTime = true;
            return true;
        }
        else
        {
            stageStates[stageId].newClearTime = false;
            return false;
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

    //クリアランク記録
    public void SetClearRank(int stageId, ClearRank rank)
    {
        stageStates[stageId].clearRank = rank;
    }

    //実績用のクリアランクチェック
    public bool ClearRankCheck()
    {
        if (stageStates[1].clearRank == ClearRank.S &&
            stageStates[2].clearRank == ClearRank.S &&
            stageStates[3].clearRank == ClearRank.S)
        {
            return true;
        }
        else { return false; }
    }
}

namespace Gamepara
{
    public enum StageType
    {
        Tutorial,
        stage1, stage2, stage3
    }

    public enum ClearRank 
    {
        B = 0,
        A = 1,
        S = 2,
    }

    public struct StagePlayTimes
    {
        public StageType stageType;
        public float stageTime;
    }
}
