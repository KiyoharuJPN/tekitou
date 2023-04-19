using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpAttack : MonoBehaviour
{
    public static void _UpAttack(Rigidbody2D player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.Skill2);
        player.AddForce(player.transform.up * skill.distance, ForceMode2D.Impulse);
        Debug.Log("êÿÇËè„Ç∞ÅI");
    }
}
