using System.Threading.Tasks;
using UnityEngine;

public class UpAttack : MonoBehaviour
{
    public static async void _UpAttack(PlayerController player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.RoundingUp);
        player.isUpAttack = true;
        player.animator.SetBool("IsUpAttack", player.isUpAttack);
        await Task.Delay(200);
        _Move(player, skill);
    }

    static async void _Move(PlayerController player, Skill skill)
    {
        player.rb.AddForce(player.transform.up * skill.distance, ForceMode2D.Impulse);
        await Task.Delay(500);
        player.isUpAttack = false;
        player.animator.SetBool("IsUpAttack", player.isUpAttack);
    }
}
