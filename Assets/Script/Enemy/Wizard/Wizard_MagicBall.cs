using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard_MagicBall : Projectile
{
    ////同時複数の魔術師の攻撃を食らうことを消すように
    //[System.NonSerialized]
    //public static bool WizardHadAttack;
    public float WMBClearTime = 10;

    //攻撃力関連
    float knockBackValue;
    int attackPower;

    //レジッドボディ
    Rigidbody2D WMBRb;
    //アニメーター（エクススキル用）
    Animator animator;

    Vector2 primarySpeed;
    bool clearWMB = true;

    private void Awake()
    {
        WMBRb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }
    private void OnEnable()
    {
        StartCoroutine(WMBClear(WMBClearTime));
        clearWMB = true;
    }

    //何かぶつかった時の処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Attack(collision);
        }

        if (clearWMB)
        {
            clearWMB = false;
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    //攻撃力とノックバック力の設定及び進行方向の初期化
    public void AKForce(int atkpower,float kbvpower,Vector2 direction)
    {
        attackPower = atkpower;
        knockBackValue = kbvpower;
        WMBRb.velocity = direction;
    }

    //攻撃時コード
    void Attack(Collider2D col)
    {
        //if (!WizardHadAttack)
        //{
        //    //攻撃クールダウンタイム
        //    WizardHadAttack = true;
        //    StartCoroutine(WHadAttackReset());
        //ダメージとノックバック
        col.gameObject.GetComponent<PlayerController>().KnockBack(this.transform.position, 15 * knockBackValue);
        col.gameObject.GetComponent<PlayerController>()._Damage(attackPower);
        //}
    }

    IEnumerator WMBClear(float time)
    {
        //var wait = 0;
        //while (wait < time)
        //{
        //    wait++;
        //    Debug.Log(wait);
        //    yield return new WaitForSeconds(0.01f);
        //}
        yield return new WaitForSeconds(time);
        
        if (clearWMB)
        {
            clearWMB = false;
            ObjectPool.Instance.PushObject(gameObject);
        }
    }

    //    //攻撃クールダウン
    //    protected IEnumerator WHadAttackReset()
    //    {
    //        var n = 20;
    //        while (n > 0)
    //        {
    //            n--;
    //            yield return new WaitForSeconds(0.01f);
    //        }
    //        WizardHadAttack = false;
    //    }


    //外部関数
    public override void EnemyStop()
    {
        isPlayerExAttack = true;
        primarySpeed = WMBRb.velocity;
        WMBRb.velocity = Vector2.zero;
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
        WMBRb.velocity = primarySpeed;
        isPlayerExAttack = false;
    }
}
