using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerSEList")]
public class PlayerSE : MonoBehaviour
{

    public void _DashSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Dash);
    }
    public void _JumpSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Jump);
    }
    public void _SecondJumpSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.AirJump);
    }

    public void _NomalAttack()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.AutoAttack);
    }

    public void _UpAttackSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.UpAttack);
    }

    public void _DownAttackStartSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.DropAttackStart);
    }

    public void _DownAttackFallSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.DropAttack);
    }

    public void _DownAttackEndSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.DropAttackLand);
    }

    public void _SideAttackSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.SideAttack);
    }

    public void _PlayerDamageSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.PlayerGetHit);
    }



    public void _ExAttack_Wind()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Wind);
    }
    
    public void _ExAttack_PowerCharge()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_PowerCharge);
    }

    public void _ExAttack_LastSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.BossDown);
    }
}
