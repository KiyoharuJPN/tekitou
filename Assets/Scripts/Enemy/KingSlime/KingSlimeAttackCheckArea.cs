using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeAttackCheckArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<KingSlime>().GetIsBlowing())
            gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            transform.GetComponentInParent<KingSlime>().PlayerInAttackArea(collision);
            //gameObject.SetActive(false);
        }
    }

}
