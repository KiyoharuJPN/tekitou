using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerController;

public class DropAttack
{
    //î≠ê∂éûÇÃî˜è„è∏íl
    const float upDistance = 5f;

    public static void DropAttackStart(PlayerController player, MonoBehaviour mono)
    {
        mono.StartCoroutine(DropAttackMove(player));
    }

    static IEnumerator DropAttackMove(PlayerController player)
    {
        player.isAttack = true;
        player.isDropAttack = true;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
        player.canDropAttack = false;
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
            player.rb.velocity = new Vector2(0, -skill.distance);
            yield return null;
        }
        player.animator.Play("Hero_DropAttack_End");
        yield return new WaitForSeconds(0.18f);

        player.enemylist.Clear();
        player.isAttack = false;
        player.isDropAttack = false;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
        player.canDropAttack = true;
    }
}
