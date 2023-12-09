using System.Collections;
using UnityEngine;

public class NomalAttack
{
    static Skill skill;
    //�󒆎��̃N�[���^�C��
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
            player.AttackEnd();
        }
    }

    public static float AttackCoolTime(PlayerController player)
    {
        if (player.isFalling || player.isJumping) //�󒆒ʏ�U���̏ꍇ
        {
            isAirAttack = true;
            return airCoolTime + AnimationCipsTime.GetAnimationTime(player.animator, AnimationCipsTime.ClipType.NomalAttack_Jump);
        }
        else
        {
            return AnimationCipsTime.GetAnimationTime(player.animator, AnimationCipsTime.ClipType.NomalAttack_Stage);
        }
    }
}
