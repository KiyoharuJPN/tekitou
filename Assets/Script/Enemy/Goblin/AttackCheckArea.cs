using UnityEngine;

public class AttackCheckArea : MonoBehaviour
{
    private void Update()
    {
        if(transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<Enemy>().PlayerInAttackArea();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player")&& transform.GetComponentInParent<Enemy>().GetIsMoving())
            transform.GetComponentInParent<Enemy>().PlayerInAttackArea();
    }
}
