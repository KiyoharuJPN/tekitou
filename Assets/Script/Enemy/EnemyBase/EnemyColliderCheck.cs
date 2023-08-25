using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderCheck : MonoBehaviour
{
    //�E���ꂽ�Ƃ��ɃR���C�_�[�̃`�F�b�N���I�t�ɂ���
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    //�g���K�[�ɓ��鎞�ɃR���C�_�[�̓��������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponentInParent<Enemy>().OnColEnter(collision);
    }

    //�g���K�[�ɂ�������Ƃ��ɃR���C�_�[�̓��������
    private void OnTriggerStay2D(Collider2D collision)
    {
        GetComponentInParent<Enemy>().OnColStay(collision);
    }
}
