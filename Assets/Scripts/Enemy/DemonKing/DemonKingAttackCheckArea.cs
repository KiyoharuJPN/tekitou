using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKingAttackCheckArea : MonoBehaviour
{

    public bool isLeftHand;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<DemonKing>().PlayerInAttackArea(collision, isLeftHand);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<DemonKing>().PlayerInAttackArea(collision, isLeftHand);
        }
    }
}
