using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技：切り上げ
public class RoundingUp : MonoBehaviour
{
    public static void _RoundingUp(Rigidbody2D player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.RoundingUp);
        player.AddForce(player.transform.up * skill.distance, ForceMode2D.Impulse);
        Debug.Log("切り上げ！");
    }
}
