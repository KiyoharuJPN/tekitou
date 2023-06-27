using System.Collections;
using UnityEngine;

public class NomalAttack
{
    static Skill skill;
    //�󒆎��̃N�[���^�C��
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
        if (player.isFalling || player.isJumping) //�󒆒ʏ�U���̏ꍇ
        {
            mono.StartCoroutine(_NomalAttackInterval(airCoolTime, player));
        }
        else mono.StartCoroutine(_NomalAttackInterval(skill.coolTime, player));
    }

    //�N�[���^�C���p�R���[�`��
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
