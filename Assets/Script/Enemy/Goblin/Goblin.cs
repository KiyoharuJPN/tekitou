using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    Animator animator;

    [Tooltip("移動速度")]
    public float movingSpeed;
    [Tooltip("待ち時間の設定")]
    public float idleTime = 2.4f;
    [SerializeField]
    [Tooltip("ゴブリン攻撃")]
    public GameObject GoblinAttack;

    //float x = 1,y = 1;

    //チェック用内部関数
    bool IsBlowing = false, IsMoving = true, IsAttacking = false, AttackChecking = true, PlayerNotAttacked = true;

    //移動速度内部関数
    float moveSpeed;        

    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;           //移動速度の方向修正
        animator = GetComponent<Animator>();    //自分用アニメーターの代入
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

    IEnumerator Attacking()
    {
        //アニメ調整
        IsMoving = false;
        IsAttacking = true;

        //攻撃するときの動き
        int i = 0;
        while (i < 10)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.55f, 1.05f);
        GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.4f, 1f);
        GoblinAttack.SetActive(true);
        while (i < 15)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-.8f, .3f);
        GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.7f, 2.3f);
        while (i < 20)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-1.05f, -0.1f);
        GoblinAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.1f, 1.6f);
        while (i < 21)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        GoblinAttack.SetActive(false);
        while (i < 30)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //アニメ調整
        IsAttacking = false;
        StartCoroutine(Idling());
    }

    IEnumerator Idling()
    {
        yield return new WaitForSeconds(idleTime);
        IsMoving = true;
        AttackChecking = true;
        PlayerNotAttacked = true;
    }

    //プレイヤーが攻撃エリアに要る時の動き（AttackCheckAreaから呼ばれる）
    public void PlayerInAttackArea()
    {
        if (IsMoving&&AttackChecking)
        {
            AttackChecking = false;
            StartCoroutine(Attacking());
        }
    }

    //外から今の移動状態を確認
    public bool GetIsMoving()
    {
        return IsMoving;
    }

    //外から今の吹き飛ばし状態確認
    public bool GetIsBlowing()
    {
        return IsBlowing;
    }

    //ゴブリンの移動方向を変える
    public void TurnAround()
    {
        bool InCheck = true;
        if(transform.localScale.x == 1f && InCheck)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            InCheck = false;
        }
        if(transform.localScale.x == -1f && InCheck)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            //InCheck = false;
        }
        moveSpeed *= -1;
    }

    //攻撃力を外で取得する
    public int GetGoblinDamage()
    {
        return enemyData.attackPower;
    }
    
    //ノックバック力を外で取得する/
    public float GetGoblinKnockBackForce()
    {
        return enemyData.knockBackValue;
    }

    public bool GetPlayerAttacked()
    {
        return PlayerNotAttacked;
    }
    public void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }
}
