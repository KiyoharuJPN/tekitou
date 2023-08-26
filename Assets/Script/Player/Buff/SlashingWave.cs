using System.Collections;
using UnityEngine;

public class SlashingWave : MonoBehaviour
{
    public PlayerController player;
    //斬撃はの生存時間
    [SerializeField, Header("生存時間")]
    float time = 2f;

    private void Start()
    {
        StartCoroutine(SlashingDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            player._Attack(collision,skill.damage, skill, false);
            Destroy(this.gameObject);
        }
    }

    IEnumerator SlashingDestroy()
    {
        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
