using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderCheck : MonoBehaviour
{
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
