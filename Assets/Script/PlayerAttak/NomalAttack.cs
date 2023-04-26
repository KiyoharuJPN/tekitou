using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class NomalAttack : MonoBehaviour
{
    GameObject player;
    PlayerController playerController;
    Skill skill;

    bool isAttack = false;
    bool isCoolTime = true;

    private void Start()
    {
        skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
        player = transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
    }


    //UŒ‚”ÍˆÍ‚É“ü‚Á‚½Žž
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true; 
    }

    //G‚ê‚Ä‚¢‚éŽž
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && isAttack && isCoolTime)
        {
            //UŒ‚
            playerController.animator.SetTrigger("IsNomalAttack");
            playerController._Attack(collision, skill.damage);
            StartCoroutine(_interval());
            isCoolTime = false;
        }
    }

    //UŒ‚”ÍˆÍ‚©‚ç“G‚ª‚¢‚È‚­‚È‚Á‚½Žž
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAttack = false;
    }

    //ƒN[ƒ‹ƒ^ƒCƒ€—pƒRƒ‹[ƒ`ƒ“
    IEnumerator _interval()
    {
        float time = skill.coolTime;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        isCoolTime = true;
    }
}
