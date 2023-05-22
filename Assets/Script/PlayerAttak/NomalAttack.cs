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
        //Žè“®UŒ‚FUŒ‚ƒ{ƒ^ƒ“‚ª‰Ÿ‚³‚ê‚¹‚½‚Æ‚«
        if (Input.GetKeyDown("joystick button 2") && isCoolTime)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            if (enemyCollision != null)
            {
                playerController._Attack(enemyCollision, skill.damage);
                ExAttackParam.Instance.AddGauge();
            }
            StartCoroutine(_interval());
        }

        //Žè“®UŒ‚FUŒ‚ƒ{ƒ^ƒ“‚ª‰Ÿ‚³‚ê‚Ä‚¢‚éŠÔ
        if (Input.GetKey("joystick button 2") && isCoolTime || Input.GetKey(KeyCode.Mouse0) && isCoolTime)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            if (enemyCollision != null)
            {
                playerController._Attack(enemyCollision, skill.damage);
                ExAttackParam.Instance.AddGauge();
            }
            StartCoroutine(_interval());
        }
    }

    //UŒ‚”ÍˆÍ‚É“ü‚Á‚½Žž
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true;
        enemyCollision = collision;
    }

    //UŒ‚”ÍˆÍ‚©‚ç“G‚ª‚¢‚È‚­‚È‚Á‚½Žž
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAttack = false;
        enemyCollision = null;
    }

    //ƒN[ƒ‹ƒ^ƒCƒ€—pƒRƒ‹[ƒ`ƒ“
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
