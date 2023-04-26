using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toge : MonoBehaviour
{
    [SerializeField]
    [Header("�_���[�W��")]
    int damage;
    [SerializeField]
    [Header("�m�b�N�o�b�N�l")]
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
