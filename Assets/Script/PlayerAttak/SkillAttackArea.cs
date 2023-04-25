using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SkillAttackArea : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    //�U���͈͂ɓ�������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //�U��
            if(player.isUpAttack)
            {
                Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.UpAttack);
                player._Attack(collision, skill.damage);
            }
            if(player.isSideAttack)
            {
                Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.SideAttack);
                player._Attack(collision, skill.damage);
            }
            if (player.isDropAttack)
            {
                Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.DropAttack);
                player._Attack(collision, skill.damage);
            }
            
        }
    }
}
