using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashingWave_Demo : SlashingWave
{
    //�a���͂̐�������
    [SerializeField, Header("��������")]
    float time = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            player.Attack(collision, skill.damage, skill, false);
        }
    }
}
