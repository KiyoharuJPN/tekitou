using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFallStone : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D bc;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(bc != null)
        {
            bc.enabled = true;
            animator.SetBool("IsBroken", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActiveAndEnabled && collision.CompareTag("Player"))
        {
            //ぶつかったら
            OnCollide();

            //プレイヤーに対する攻撃
            //ダメージとノックバック
            collision.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * 4);
            collision.gameObject.GetComponent<PlayerController>()._Damage(2);
        }

        if(isActiveAndEnabled && collision.CompareTag("Stage"))
        {
            //ぶつかったら
            OnCollide();
        }
    }

    //外部関数
    public void SetSpeed(float FallSpeed)
    {
        rb.velocity = new Vector2(0, -FallSpeed);
    }

    
    //内部関数
    void DestroyThis()
    {
        ObjectPool.Instance.PushObject(gameObject);
    }

    void OnCollide()
    {
        bc.enabled = false;
        animator.SetBool("IsBroken", true);
    }
}
