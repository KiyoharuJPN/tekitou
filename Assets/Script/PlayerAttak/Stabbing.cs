using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�Z�F�˂��h��
class Stabbing : MonoBehaviour
{
    public static void _Stabbing(Rigidbody2D player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.Stabbing);
        player.AddForce(-player.transform.up * skill.distance, ForceMode2D.Impulse);
        Debug.Log("�˂��h���I");
    }
}
