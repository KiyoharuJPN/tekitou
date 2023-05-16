using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GetComponentInParent<Goblin>().GetPlayerAttacked())
        {
            GetComponentInParent<Goblin>().SetPlayerAttacked(false);
            collision.GetComponent<PlayerController>().KnockBack(transform.position, GetComponentInParent<Goblin>().GetGoblinKnockBackForce());
            collision.GetComponent<PlayerController>()._Damage(GetComponentInParent<Goblin>().GetGoblinDamage());
        }
    }
}
