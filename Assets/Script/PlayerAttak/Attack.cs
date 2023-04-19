using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    [Header("�A�^�b�N�A�j���[�V����")]
    AnimationClip attackClip;

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


    //�U���͈͂ɓ�������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        isAttack = true; 
    }

    //�G��Ă��鎞
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("�U���\");
        if (collision.CompareTag("Enemy") && isAttack && isCoolTime)
        {
            //�U��
            playerController._Attack(collision);
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
