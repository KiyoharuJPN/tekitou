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


    //攻撃範囲に入った時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true; 
    }

    //触れている時
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && isAttack && isCoolTime)
        {
            //攻撃
            playerController.animator.SetTrigger("IsNomalAttack");
            playerController._Attack(collision, skill.damage);
            StartCoroutine(_interval());
            isCoolTime = false;
        }
    }

    //攻撃範囲から敵がいなくなった時
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAttack = false;
    }

    //クールタイム用コルーチン
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
