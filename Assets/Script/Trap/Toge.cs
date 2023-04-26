using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toge : MonoBehaviour
{
    [SerializeField]
    [Header("ダメージ量")]
    int damage;
    [SerializeField]
    [Header("ノックバック値")]
    float knockBackValue;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            collision.gameObject.GetComponent<PlayerController>().KnockBack(1, this.transform.position, 15 * knockBackValue);
            collision.gameObject.GetComponent<PlayerController>()._Damage(damage);
        }
    }
}
