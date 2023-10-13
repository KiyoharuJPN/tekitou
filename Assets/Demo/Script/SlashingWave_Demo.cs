using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashingWave_Demo : SlashingWave
{
    //斬撃はの生存時間
    [SerializeField, Header("生存時間")]
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
