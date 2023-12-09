using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

public class DropAttack
{
    //”­¶Žž‚Ì”÷ã¸’l
    const float upDistance = 5f;

    public static void DropAttackStart(PlayerController player, MonoBehaviour mono)
    {
        mono.StartCoroutine(DropAttackMove(player));
    }

    static IEnumerator DropAttackMove(PlayerController player)
    {
        float time = 0;
        player.animator.SetBool("IsDropAttack", true);
        player.animator.Play("Hero_DropAttack_Start");
        player.rb.velocity = new Vector2(0, 0);
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);

        if (player.isGround)
        {
            player.rb.AddForce(player.transform.up * upDistance, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        player.rb.velocity = Vector2.zero;

        while (!player.isGround)
        {
            time += Time.deltaTime;
            player.rb.velocity = new Vector2(0, -skill.distance);
            yield return null;

            if(time > 3f)
            {
                player.animator.Play("Hero_DropAttack_End");
                player.canDropAttack = true;
                player.AttackEnd();
                break;
            }
        }

    }
}
