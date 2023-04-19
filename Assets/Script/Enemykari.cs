using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Enemykari : MonoBehaviour
{
    [Tooltip("ノックバック値")]
    public float knockBackValue;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<PlayerController>().KnockBack(1, this.transform.position, 15 * knockBackValue);
    }
}
