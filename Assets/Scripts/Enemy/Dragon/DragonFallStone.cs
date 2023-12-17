using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFallStone : Projectile
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
            collision.gameObject.GetComponent<PlayerController>().Damage(2);
        }

        if(isActiveAndEnabled && (collision.CompareTag("Stage") || collision.CompareTag("PlatFormStage")))
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
        //ObjectPool.Instance.PushObject(gameObject);
        Destroy(gameObject);
    }

    void OnCollide()
    {
        bc.enabled = false;
        animator.SetBool("IsBroken", true);
    }

    //外部関数
    public override void EnemyStop()
    {
        isPlayerExAttack = true;
        primarySpeed = rb.velocity;
        rb.velocity = Vector2.zero;
        if (animator != null)
        {
            animator.speed = 0;
        }
    }
    public override void Stop_End()
    {
        isPlayerExAttack = false;
        if (animator != null)
        {
            animator.speed = 1;
        }
        rb.velocity = primarySpeed;
        isPlayerExAttack = false;
        Debug.Log("enemyStopEnd");
    }

}
