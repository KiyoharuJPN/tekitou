using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashingWave_Demo : MonoBehaviour
{
    public PlayerController player;
    //ŽaŒ‚‚Í‚Ì¶‘¶ŽžŠÔ
    [SerializeField, Header("¶‘¶ŽžŠÔ")]
    float time = 2f;

    private void Start()
    {
        StartCoroutine(SlashingDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            player.Attack(collision, skill.damage, skill, false);
        }
    }

    IEnumerator SlashingDestroy()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
