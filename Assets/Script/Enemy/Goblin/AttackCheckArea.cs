using UnityEngine;

public class AttackCheckArea : MonoBehaviour
{
    private void Update()
    {
        if(transform.GetComponentInParent<Goblin>().GetIsBlowing())
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<Goblin>().PlayerInAttackArea();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player")&& transform.GetComponentInParent<Goblin>().GetIsMoving())
            transform.GetComponentInParent<Goblin>().PlayerInAttackArea();
    }
}
