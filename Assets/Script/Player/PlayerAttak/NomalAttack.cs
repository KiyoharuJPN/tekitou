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

    Collider2D enemyCollision;

    private void Start()
    {
        skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
        player = transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        //手動攻撃：攻撃ボタンが押されせたとき
        if (Input.GetKeyDown("joystick button 2") && isCoolTime)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            if (enemyCollision != null)
            {
                playerController._Attack(enemyCollision, skill.damage);
                ExAttackParam.Instance.AddGauge();
                playerController._HitEfect(enemyCollision.transform, skill.hitEffectAngle);
            }
            StartCoroutine(_interval());
        }

        //手動攻撃：攻撃ボタンが押されている間
        if (Input.GetKey("joystick button 2") && isCoolTime || Input.GetKey(KeyCode.Mouse0) && isCoolTime)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            if (enemyCollision != null)
            {
                playerController._Attack(enemyCollision, skill.damage);
                ExAttackParam.Instance.AddGauge();
                playerController._HitEfect(enemyCollision.transform, skill.hitEffectAngle);
            }
            StartCoroutine(_interval());
        }
    }

    //攻撃範囲に入った時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true;
        enemyCollision = collision;
    }

    //攻撃範囲から敵がいなくなった時
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAttack = false;
        enemyCollision = null;
    }

    //クールタイム用コルーチン
    IEnumerator _interval()
    {
        float time = skill.coolTime;
        isCoolTime = false;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        isCoolTime = true;
    }
}
