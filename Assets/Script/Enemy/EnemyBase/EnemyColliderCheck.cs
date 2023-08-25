using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderCheck : MonoBehaviour
{
    //殺されたときにコライダーのチェックをオフにする
    private void Update()
    {
        if (transform.GetComponentInParent<Enemy>().GetIsBlowing())
            gameObject.SetActive(false);
    }

    //トリガーに入る時にコライダーの動作をする
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponentInParent<Enemy>().OnColEnter(collision);
    }

    //トリガーにい続けるときにコライダーの動作をする
    private void OnTriggerStay2D(Collider2D collision)
    {
        GetComponentInParent<Enemy>().OnColStay(collision);
    }
}
