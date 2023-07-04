using UnityEngine;

//中間地点
public class WayPoint : MonoBehaviour
{
    Animator animator;

    bool isWayPoint = false;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();

        isWayPoint = SceneData.Instance.wayPoint;

        //開始時に中間起動済み状態の場合
        if (isWayPoint)
        {
            animator.SetTrigger("IsWayPoint");
            this.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isWayPoint) 
        {
            isWayPoint = true;
            animator.SetTrigger("IsWayPoint");
            SceneData.Instance.wayPoint = true;
            this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            this.enabled = false;
        }
    }
}
