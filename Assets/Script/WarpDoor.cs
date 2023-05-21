using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] internal Animator animator;

    GameObject warpPoint;

    private void Start()
    {
        warpPoint = transform.Find("WarpPoint").gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //�Փ˂��Ă��镨�̃��C���[��Player(6�ԃ��C���[�j�łȂ���� return����
        if (collision.gameObject.layer != 6) return;

        float lsv = Input.GetAxis("L_Stick_V");
        if (lsv >= 0.8)
        {
            animator.SetTrigger("DoorOpen");

            //TODO�@���݂ł̓t�F�[�h�C���𖢎����̈׃R���[�`���Ŏ����A�C���\��
            StartCoroutine(PlayerWarp(3.0f, collision));
        }
    }

    IEnumerator PlayerWarp(float delay,Collider2D collider)
    {
        yield return new WaitForSeconds(delay);
        collider.transform.position = warpPoint.transform.position;
    }
}
