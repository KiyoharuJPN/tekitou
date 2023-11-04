using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    [System.Serializable]
    struct MoveWallStatus
    {
        [SerializeField, Header("�v���C���[")]
        public GameObject player;

        [SerializeField, Header("�ړ��X�s�[�h ����n�ȉ��̏ꍇ")]
        public float moveSpeed_below;
        [SerializeField, Header("�ړ��X�s�[�h ����n�ȏ�̏ꍇ")]
        public float moveSpeed_more;
        [SerializeField, Header("����")]
        public float distance;

        [SerializeField, Header("���B�n�_X���l")]
        public float xNum;
    }

    [SerializeField, Header("�����ǐݒ�")]
    MoveWallStatus moveWallStatus;

    private void Start()
    {
        MoveStart();
    }

    public void MoveStart()
    {
        StartCoroutine(MoveCheck());
    }

    IEnumerator MoveCheck()
    {
        Vector2 targetPos = new Vector2(moveWallStatus.xNum, transform.position.y);
        var dis = Vector2.Distance(transform.position, targetPos);
        while (dis > 0.1f)
        {
            if(moveWallStatus.player.GetComponent<PlayerController>().GetIsDead) { break; }
            Move(targetPos);
            dis = Vector2.Distance(transform.position, targetPos);
            yield return null;
        }

        MoveEnd();
    }

    void Move(Vector2 movePos)
    {
        this.transform.position = Vector2.MoveTowards(transform.position, movePos, MoveSpeedSet() * Time.deltaTime); // �ړI�̈ʒu�Ɉړ�
    }

    float MoveSpeedSet()
    {
        //�w�肳��Ă��鋗�����Player�Ƃ̋���������ꍇ
        if (Vector2.Distance(transform.position, moveWallStatus.player.transform.position) >= moveWallStatus.distance)
        {
            return moveWallStatus.moveSpeed_more;
        }
        else
        {
            Debug.Log(moveWallStatus.moveSpeed_below);
            return moveWallStatus.moveSpeed_below;
        }
    }

    void MoveEnd()
    {
    }
}
