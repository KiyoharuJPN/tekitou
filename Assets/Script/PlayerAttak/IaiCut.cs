using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//技：居合切り
public class IaiCut : MonoBehaviour
{
    public static void _IaiCut(Rigidbody2D player)
    {
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.IaiCut);


        
        Debug.Log("居合切り！");
    }

}
