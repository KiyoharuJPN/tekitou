using UnityEngine;

public class Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GetComponentInParent<Enemy>().GetPlayerAttacked())
        {
            GetComponentInParent<Enemy>().SetPlayerAttacked(false);
            collision.GetComponent<PlayerController>().KnockBack(transform.position, GetComponentInParent<Enemy>().GetKnockBackForce());
            collision.GetComponent<PlayerController>()._Damage(GetComponentInParent<Enemy>().GetDamage());
        }
    }
}
