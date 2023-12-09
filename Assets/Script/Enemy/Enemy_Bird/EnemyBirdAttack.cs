using UnityEngine;

public class EnemyBirdAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GetComponentInParent<Enemy_Bird>().GetPlayerAttacked())
        {
            if (GetComponentInParent<Enemy_Bird>().HadAttacked())
            {
                GetComponentInParent<Enemy_Bird>().SetPlayerAttacked(false);
                collision.GetComponent<PlayerController>().KnockBack(transform.position, GetComponentInParent<Enemy_Bird>().GetKnockBackForce());
                collision.GetComponent<PlayerController>().Damage(GetComponentInParent<Enemy_Bird>().GetDamage());
            }
        }
    }
}
