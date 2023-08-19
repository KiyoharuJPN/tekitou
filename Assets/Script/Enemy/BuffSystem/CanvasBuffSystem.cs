using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBuffSystem : MonoBehaviour
{
    [SerializeField, Tooltip("�A������")]
    float ComboKillTime = 10;
    float Slashingcombokilltime, invinciblecombokilltime;

    //�擾��
    int SkillGaugeUpAcquisitionTimes, SpeedUpAcquisitionTimes,
        SlashingAcquisitionTimes, invincibleAcquisitionTimes;

    private void Start()
    {
        
    }


    //�擾�񐔂𑝉�����֐�
    public void AddComboKill(int bufftype)
    {
        
    }

    //�擾�񐔂̃Q�b�g�֐�
    public int GetAcquisitionTimes(int bufftype)
    {
        switch (bufftype)
        {
            case 0:
                return SkillGaugeUpAcquisitionTimes;
            case 1:
                return SpeedUpAcquisitionTimes;
            case 3:
                return SlashingAcquisitionTimes;
            case 4:
                return invincibleAcquisitionTimes;
            default:
                return 0;
        }
    }

    //�����񐔂̃Q�b�g�֐�
    public int GetIncrementCount(int bufftype)
    {
        switch (bufftype)
        {
            case 0:
                return SkillGaugeUpAcquisitionTimes / 2;
            case 1:
                return SpeedUpAcquisitionTimes / 2;
            case 3:
                return SlashingAcquisitionTimes / 2;
            case 4:
                return invincibleAcquisitionTimes / 2;
            default:
                return 0;
        }
    }

    public int GetIncrementSpeed(int bufftype)
    {
        switch (bufftype)
        {
            case 0:
                return SkillGaugeUpAcquisitionTimes * 2;
            case 1:
                return SpeedUpAcquisitionTimes * 2;
            case 3:
                return SlashingAcquisitionTimes * 2;
            case 4:
                return invincibleAcquisitionTimes * 2;
            default:
                return 0;
        }
    }
}
