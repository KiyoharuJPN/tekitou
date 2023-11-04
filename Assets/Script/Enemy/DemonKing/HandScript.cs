using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    DemonKing demonKing;

    private void Start()
    {
        demonKing = GetComponentInParent<DemonKing>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerEnter2D(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(demonKing != null)
        {
            demonKing.OnHandTriggerStay2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerExit2D(collision);
        }
    }

    private void StopHandAnimation()
    {
        GetComponent<Animator>().speed = 0;
    }
}
