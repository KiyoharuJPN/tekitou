using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//‹ZF“Ë‚«h‚µ
class Stabbing : MonoBehaviour
{
    public static void _Stabbing(Rigidbody2D player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.Stabbing);
        player.AddForce(-player.transform.up * skill.distance, ForceMode2D.Impulse);
        Debug.Log("“Ë‚«h‚µI");
    }
}
