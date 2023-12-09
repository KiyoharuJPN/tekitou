using System.Collections;
using UnityEngine;

public class NomalAttack
{
    static Skill skill;
    //空中時のクールタイム
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

    //クールタイム用コルーチン
    static IEnumerator NomalAttackInterval(PlayerController player, MonoBehaviour mono)
    {
        float time = AttackCoolTime(player);

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        if (player.isNomalAttackKay && !isAirAttack)//アタック再使用確認
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
        if (player.isFalling || player.isJumping) //空中通常攻撃の場合
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
