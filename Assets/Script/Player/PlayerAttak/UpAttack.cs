using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class UpAttack
{
    private static Skill upAttackStatus;

    public static async void UpAttackStart(PlayerController player, Player_Jump p_Jump, MonoBehaviour mono)
    {
        upAttackStatus = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
        player.animator.SetBool("IsUpAttack", true);
        player.animator.Play("Hero_UpAttack_Start");
        p_Jump.jumpPos = player.transform.position.y;
        p_Jump.jumpHight = 3f;
        await Task.Delay(170);
        player.rb.velocity = new Vector2(0, 0);
        mono.StartCoroutine(UpAttackTime(player,p_Jump,mono));
    }

    private static void UpAttackMove(PlayerController player, Player_Jump p_Jump)
    {
        player.rb.velocity = new Vector2(0, p_Jump.HeigetLimt(p_Jump.jumpPos, p_Jump.jumpHight, upAttackStatus.distance) + p_Jump.jumpTime * Time.deltaTime);
    }

    private static IEnumerator UpAttackTime(PlayerController player, Player_Jump p_Jump, MonoBehaviour mono)
    {
        var time = upAttackStatus.activeTime;

        while (time > 0)
        {
            if (player.isWarpDoor) break;
            UpAttackMove(player, p_Jump);
            time -= Time.deltaTime;

            //イベント時は終了
            if (player.playerState == PlayerController.PlayerState.Event)
            {
                player.animator.SetBool("IsUpAttack", false);
                break;
            }
            yield return null;
        }
        player.rb.velocity = new Vector2(player.rb.velocity.x, player.rb.velocity.y / 3);
        player.AttackEnd();
    }

    public static void UpAttackEnd(PlayerController player, Player_Jump p_Jump)
    {
        player.AttackEnd();
    }
}
