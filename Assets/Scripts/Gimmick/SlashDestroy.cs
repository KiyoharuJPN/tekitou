using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashDestroy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 21)
        {
            Destroy(collision.gameObject);
        }
    }
}
