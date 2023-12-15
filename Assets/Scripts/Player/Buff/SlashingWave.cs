using System.Collections;
using UnityEngine;

public class SlashingWave : MonoBehaviour
{
    public PlayerController player;
    //�a���͂̐�������
    [SerializeField, Header("��������")]
    float time = 2f;
    [SerializeField, Header("���Ŏ��̃f�B���C")]
    float destroyDelay = 0.1f;

    private void Start()
    {
        StartCoroutine(SlashingDestroy());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
            player.Attack(collision,skill.damage, skill, false);
        }
        if(collision.gameObject.layer == 20)
        {
            Invoke("Destroy", destroyDelay);
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

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
