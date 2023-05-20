using UnityEngine;

public class GoblinAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GetComponentInParent<Goblin>().GetPlayerAttacked())
        {
            GetComponentInParent<Goblin>().SetPlayerAttacked(false);
            collision.GetComponent<PlayerController>().KnockBack(transform.position, GetComponentInParent<Goblin>().GetKnockBackForce());
            collision.GetComponent<PlayerController>()._Damage(GetComponentInParent<Goblin>().GetDamage());
        }
    }
}
