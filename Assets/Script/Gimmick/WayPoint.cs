using UnityEngine;
using static Item;

//���Ԓn�_
public class WayPoint : MonoBehaviour
{
    Animator animator;

    bool isWayPoint = false;

    //���Ԓn�_����
    public int pointNum; 

    private void Awake()
    {
        animator = this.GetComponent<Animator>();

        if(pointNum == 1)
        {
            isWayPoint = SceneData.Instance.wayPoint_1;
        }
        else if(pointNum == 2)
        {
            isWayPoint = SceneData.Instance.wayPoint_2;
        }

        //�J�n���ɒ��ԋN���ςݏ�Ԃ̏ꍇ
        if (isWayPoint)
        {
            animator.SetTrigger("IsWayPoint");
            this.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!isWayPoint && (col.CompareTag("Player") || col.CompareTag("InvinciblePlayer"))) 
        {
            animator.SetTrigger("IsWayPoint");
            if (pointNum == 1)
            {
                SceneData.Instance.wayPoint_1 = true;
            }
            else if (pointNum == 2)
            {
                SceneData.Instance.wayPoint_2 = true;
            }
            SoundManager.Instance.PlaySE(SESoundData.SE.HalfPoint);
            if(col != null)
            {
                col.GetComponent<PlayerController>().Heel(4);
            }
            isWayPoint = true;
            this.enabled = false;
        }
    }
}
