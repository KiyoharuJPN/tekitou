
using System;
using Gamepara;

public class SceneData
{
    public readonly static SceneData Instance = new();

    //�V�[���̖��O�L�^
    public string referer = string.Empty;

    //�v���C���[�c�@
    public int stock = 2;
    public static int MAX_STOCK = 2;

    //�������m�F�pBool
    public bool revival = false;

    //���Ԓn�_�N�����
    public bool wayPoint_1 = false;
    public bool wayPoint_2 = false;

    public void DataReset()
    {
        stock = MAX_STOCK;
        wayPoint_1 = false;
        wayPoint_2 = false;
    }

    //�v���C����
    public float playTime;
    public float stage1Time;
    public float stage2Time;
    public float stage3Time;

    //�v���C���ԋL�^
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
