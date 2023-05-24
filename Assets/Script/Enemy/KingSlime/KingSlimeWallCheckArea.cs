using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSlimeWallCheckArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
            GetComponentInParent<KingSlime>().TurnAround();
    }
}
