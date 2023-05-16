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


    //�U���͈͂ɓ�������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true;
    }

    private void Update()
    {
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (Input.GetKeyDown("joystick button 2") && isCoolTime && !playerController.isNomalAttackKay)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            if (enemyCollision != null)
            {
                playerController._Attack(enemyCollision, skill.damage);
                
            }
            StartCoroutine(_interval());
        }

        //�蓮�U���F�U���{�^����������Ă����
        if (Input.GetKey("joystick button 2") && isCoolTime && !playerController.isNomalAttackKay)
        {
            playerController.animator.SetTrigger("IsNomalAttack");
            if (enemyCollision != null)
            {
                playerController._Attack(enemyCollision, skill.damage);

            }
            StartCoroutine(_interval());
        }
    }

    //�G��Ă��鎞
    private void OnTriggerStay2D(Collider2D collision)
    {
        enemyCollision = collision;
        if (collision.CompareTag("Enemy") && isAttack && isCoolTime && playerController.isNomalAttackKay)
        {
            //�U��
            playerController.animator.SetTrigger("IsNomalAttack");
            playerController._Attack(collision, skill.damage);
            StartCoroutine(_interval());
        }
    }

    //�U���͈͂���G�����Ȃ��Ȃ�����
    private void OnTriggerExit2D(Collider2D collision)
    {
        isAttack = false;
        enemyCollision = null;
    }

    //�N�[���^�C���p�R���[�`��
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
