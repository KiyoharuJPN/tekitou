using System.Threading.Tasks;
using UnityEngine;

//ãZÅFìÀÇ´éhÇµ
class Stabbing : MonoBehaviour
{
    public static async void _Stabbing(PlayerController player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.Stabbing);
        player.isDropAttack = true;
        player.animator.SetBool("IsDropAttack", player.isDropAttack);
        await Task.Delay(200);
        _Move(player, skill);
    }

    static void _Move(PlayerController player, Skill skill)
    {
        player.rb.AddForce(-player.transform.up * skill.distance, ForceMode2D.Impulse);
    }
}
