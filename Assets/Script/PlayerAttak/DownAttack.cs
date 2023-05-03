using System.Threading.Tasks;
using UnityEngine;

public class DownAttack : MonoBehaviour
{
    public static async void _DownAttack(PlayerController player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
        player.isDropAttack = true;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
        await Task.Delay(200);
        _Move(player, skill);
    }

    static async void _Move(PlayerController player, Skill skill)
    {
        player.rb.velocity = new Vector2(0, 0);
        await Task.Delay(200);
        player.rb.AddForce(-player.transform.up * skill.distance, ForceMode2D.Impulse);
    }
}
