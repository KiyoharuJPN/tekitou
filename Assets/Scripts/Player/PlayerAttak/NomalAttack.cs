using System.Collections;
using UnityEngine;

public class NomalAttack
{
    static Skill skill;

    //�󒆎��̒ǉ��N�[���^�C��
    const float airCoolTime = 0.1f;

    static bool isAirAttack;

    public static void NomalAttackStart(PlayerController player, MonoBehaviour mono)
    {
        player.enemylist.Clear();
        skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
        player.animator.SetBool("IsNomalAttackBool", true);
        player.animator.SetTrigger("IsNomalAttack");
        isAirAttack = false;
        mono.StartCoroutine(NomalAttackInterval(player, mono));
    }

    //�N�[���^�C���p�R���[�`��
    static IEnumerator NomalAttackInterval(PlayerController player, MonoBehaviour mono)
    {
        float time = AttackCoolTime(player);

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if (player.isNomalAttackKay && !isAirAttack)//�A�^�b�N�Ďg�p�m�F
        {
            NomalAttackStart(player, mono);
        }
        else
        {
            player.animator.SetBool("IsNomalAttackBool", false);
            player.AttackEnd();
        }
    }

    public static float AttackCoolTime(PlayerController player)
    {
        if (player.isFalling || player.isJumping) //�󒆒ʏ�U���̏ꍇ
        {
            isAirAttack = true;
            return airCoolTime + GetAnimationClipTime.GetAnimationTime(player.animator, GetAnimationClipTime.ClipType.NomalAttack_Jump) / player.animSpeed;
        }
        else
        {
            return GetAnimationClipTime.GetAnimationTime(player.animator, GetAnimationClipTime.ClipType.NomalAttack_Stage) / player.animSpeed;
        }
    }
}
