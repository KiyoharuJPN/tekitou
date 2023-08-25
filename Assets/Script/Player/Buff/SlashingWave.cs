using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlashingWave : MonoBehaviour
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
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy") || 
            collision.gameObject.layer == LayerMask.NameToLayer("PinBallEnemy") ||
            collision.gameObject.layer == LayerMask.NameToLayer("BossEnemy"))
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            player._Attack(collision, 1, skill);
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
