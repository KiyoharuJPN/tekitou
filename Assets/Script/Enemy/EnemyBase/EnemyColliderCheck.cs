using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponentInParent<Enemy>().OnColEnter(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        GetComponentInParent<Enemy>().OnColStay(collision);
    }
}
