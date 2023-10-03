using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    [Tooltip("移動速度")]
    public float movingSpeed;
    [Tooltip("待ち時間の設定")]
    public float idleTime = 2.4f;
    //[SerializeField]
    //[Tooltip("ゴブリン攻撃")]
    //public GameObject GoblinAttack;

    //float x = 1,y = 1;

    //チェック用内部関数
    //bool AttackChecking = true, PlayerNotAttacked = true;


    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;           //移動速度の方向修正
        base.Start();                           //敵のscriptに基づく
    }
    protected override void Update()
    {
        //画面内にある
        if (OnCamera)
        {
            //飛ばされてない限り
            if (!isDestroy)
            {
                Movement();
            }
        }
        //アニメ
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //状態の変更
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //敵のscriptに基づく
        base.Update();
    }

    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        Gravity();
    }

    //ゴブリンの動き
    void Movement()
    {
        //崖判断外部による崖の判断を行われています。
        /*if (!Physics.Linecast(new Vector2(transform.position.x - x + 0.01f, transform.position.y - y + 0.01f), new Vector2(transform.position.x - x, transform.position.y - y))) Debug.Log("1111111111111111111111111111111111111111111111111111"); //moveSpeed *= -1;
        Debug.Log(new Vector2(transform.position.x - x + 0.01f, transform.position.y - y + 0.01f));
        Debug.Log("\n"+new Vector2(transform.position.x - x, transform.position.y - y));*/

        //移動
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    void Attacking()
    {
        //アニメ調整
        IsMoving = false;
        IsAttacking = true;

        //攻撃するときの動き
        //int i = 0;
        //while (i < 25)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //SoundManager.Instance.PlaySE(SESoundData.SE.SwingDownClub);
        //GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.55f, 1.05f);
        //GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 1f);
        //GoblinAttack.SetActive(true);
        //while (i < 32)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.8f, .3f);
        //GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.7f, 2.3f);
        //while (i < 37)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-1.05f, -0.1f);
        //GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.6f);
        //while (i < 38)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}
        //GoblinAttack.SetActive(false);
        //while (i < 57)
        //{
        //    i++;
        //    yield return new WaitForSeconds(0.01f);
        //}

        
    }

    public void EndAttack()
    {
        //アニメ調整
        IsAttacking = false;
        StartCoroutine(Idling());
    }

    public void AttackSE()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.SwingDownClub);
    }

    IEnumerator Idling()
    {
        yield return new WaitForSeconds(idleTime);
        IsMoving = true;
        AttackChecking = true;
        PlayerNotAttacked = true;
    }

    //プレイヤーが攻撃エリアにいる時の動き（AttackCheckAreaから呼ばれる）
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        if (IsMoving&&AttackChecking)
        {
            AttackChecking = false;
            Attacking();
            InAttack = true;
        }
        return InAttack;
    }

    public override bool GetPlayerAttacked()
    {
        return PlayerNotAttacked;
    }
    public override void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }
}
