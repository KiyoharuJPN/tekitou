using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class UpAttack
{
    private static Skill upAttackStatus;

    public static async void UpAttackStart(PlayerController player, Player_Jump p_Jump, MonoBehaviour mono)
    {
        player.canUpAttack = false;
        player.isAttack = true;
        upAttackStatus = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
        player.isUpAttack = true;
        player.animator.SetBool("IsUpAttack", player.isUpAttack);
        p_Jump.jumpPos = player.transform.position.y;
        p_Jump.jumpHight = 3f;
        await Task.Delay(170);
        p_Jump.isUpAttack = true;
        player.rb.velocity = new Vector2(0, 0);
        mono.StartCoroutine(UpAttackTime(player,p_Jump));
    }

    private static void UpAttackMove(PlayerController player, Player_Jump p_Jump)
    {
        player.rb.velocity = new Vector2(0, p_Jump.HeigetLimt(p_Jump.jumpPos, p_Jump.jumpHight, upAttackStatus.distance) + p_Jump.jumpTime * Time.deltaTime);
    }

    public static void UpAttackEnd(PlayerController player, Player_Jump p_Jump)
    {
        if (p_Jump.isUpAttack)
        {
            p_Jump.isUpAttack = false;
            player.isUpAttack = false;
            player.animator.SetBool("IsUpAttack", player.isUpAttack);
        }
    }

    private static IEnumerator UpAttackTime(PlayerController player, Player_Jump p_Jump)
    {
        var time = 0.4f;

        while (time > 0)
        {
            UpAttackMove(player, p_Jump);
            time -= Time.deltaTime;
            yield return null;
        }

        UpAttackEnd(player, p_Jump);
    }
}
