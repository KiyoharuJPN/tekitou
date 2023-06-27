using System.Collections;
using UnityEngine;

public class NomalAttack
{
    static Skill skill;
    //空中時のクールタイム
    const float airCoolTime = 0.5f;

    public static void NomalAttackStart(PlayerController player)
    {
        skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
        player.canNomalAttack = false;
        player.animator.SetTrigger("IsNomalAttack");
        player.isAttack = true;
    }

    public static void AttackCool(PlayerController player, MonoBehaviour mono)
    {
        if (player.isFalling || player.isJumping) //空中通常攻撃の場合
        {
            mono.StartCoroutine(_NomalAttackInterval(airCoolTime, player));
        }
        else mono.StartCoroutine(_NomalAttackInterval(skill.coolTime, player));
    }

    //クールタイム用コルーチン
    static IEnumerator _NomalAttackInterval(float coolTime, PlayerController player)
    {
        float time = coolTime;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        player.isAttack = false;
        player.enemylist.Clear();
        player.canNomalAttack = true;
    }
}
