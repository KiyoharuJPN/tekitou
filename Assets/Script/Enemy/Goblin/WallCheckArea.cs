using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheckArea : MonoBehaviour
{
    private void Update()
    {
        if (transform.GetComponentInParent<Goblin>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stage"))
            GetComponentInParent<Goblin>().TurnAround();
    }
}
