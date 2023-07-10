using System.Threading.Tasks;
using UnityEngine;

public class DropAttack
{
    //î≠ê∂éûÇÃî˜è„è∏íl
    const float upDistance = 5f;

    public static async void DropAttackStart(PlayerController player)
    {
        player.isAttack = true;
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
        player.isDropAttack = true;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
        player.rb.velocity = new Vector2(0, 0);
        player.rb.AddForce(player.transform.up * upDistance, ForceMode2D.Impulse);
        player.canDropAttack = false;
        await Task.Delay(300);
        _Move(player, skill);
    }

    public static async void _Move(PlayerController player, Skill skill)
    {
        player.rb.velocity = new Vector2(0, 0);
        await Task.Delay(200);
        player.rb.AddForce(-player.transform.up * skill.distance, ForceMode2D.Impulse);
    }
}
