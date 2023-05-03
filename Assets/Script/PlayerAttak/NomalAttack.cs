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

    Collider2D enemyCollider2D;

    private void Start()
    {
        skill = SkillGenerater.instance.SkillSet(Skill.Type.NormalAttack);
        player = transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
    }


    //�U���͈͂ɓ�������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown("joystick button 2") && isCoolTime && !playerController.isNomalAttackKay)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            //playerController._Attack(, skill.damage);
            StartCoroutine(_interval());
            isCoolTime = false;
        }
    }

    //�G��Ă��鎞
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && isAttack && isCoolTime && playerController.isNomalAttackKay)
        {
            //�U��
            playerController.animator.SetTrigger("IsNomalAttack");
            playerController._Attack(collision, skill.damage);
            StartCoroutine(_interval());
            isCoolTime = false;
        }
        
    }

    //�U���͈͂���G�����Ȃ��Ȃ�����
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAttack = false;
    }

    //�N�[���^�C���p�R���[�`��
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
