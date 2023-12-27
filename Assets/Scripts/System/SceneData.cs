using Gamepara;
using System;
using System.Diagnostics;

public class SceneData
{
    public readonly static SceneData Instance = new();

    public struct EachStageState
    {
        public bool openStage;�@//�X�e�[�W���V�ׂ邩
        public bool isClear;  �@//�N���A�m�F
        public bool firstOpen;�@//�J���̂����߂Ă� 
        public float clearTime; //�N���A�^�C��
    }

    //�V�[���̖��O�L�^
    public string referer = string.Empty;

    private EachStageState[] stageStates = new EachStageState[Enum.GetNames(typeof(StageType)).Length];
    private bool[] stageFirstOpen = new bool[4] { false,false,true,true };

    public EachStageState[] GetEachStageState
    {
        get { return this.stageStates; }
    }

    //�v���C���[�c�@
    public int stock = 2;
    public static int MAX_STOCK = 2;

    //�������m�F�pBool
    public bool revival = false;

    //���Ԓn�_�N�����
    public bool wayPoint_1 = false;
    public bool wayPoint_2 = false;

    public void StageOpen(StageType stageType)
    {
        int stageId = (int)stageType;

        stageStates[stageId].isClear = true;

        if (stageId < stageStates.Length - 1)
        {
            stageId++;
            //���̃X�e�[�W���V�ׂ��ԂłȂ���ΕύX
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

    //�X�e�[�W��ԃ��Z�b�g
    public void StageDataReset()
    {
        stock = MAX_STOCK;
        wayPoint_1 = false;
        wayPoint_2 = false;
    }

    //�v���C���Ԉꎞ�i�[�p
    public float playTime;

    //�v���C���Ԏ擾
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
    //�v���C���ԋL�^
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
    //�w�肵���X�e�[�W�̃v���C���ԃ��Z�b�g
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
    //�S�X�e�[�W�̃v���C���ԃ��Z�b�g
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
