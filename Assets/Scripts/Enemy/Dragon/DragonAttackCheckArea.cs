using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttackCheckArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<Dragon>().PlayerInAttackArea(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<Dragon>().PlayerInAttackArea(collision);
        }
    }
}
