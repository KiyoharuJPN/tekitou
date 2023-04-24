using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]

public class Enemy : MonoBehaviour
{
    [SerializeField]
    string id;
    EnemyData enemyData;
    Rigidbody2D enemyRb;

    float hp;

    //吹っ飛び角度
    float forceAngle;
    Vector2 forceDirection = new Vector3(1.0f, 1.0f);

    [SerializeField]
    [Header("吹っ飛び速度")]
    float speed;
    [SerializeField]
    [Header("反射回数")]
    int num;

    enum moveType
    {
        NotMove, //動かない
        Move,    //動く
        FlyMove　//飛ぶ
    }
    moveType type;

    bool isDestroy;

    private void Start()
    {
        //idで指定した敵データ読込
        enemyData = EnemyGeneratar.instance.EnemySet(id);
        hp = enemyData.hp;
        enemyRb = GetComponent<Rigidbody2D>();
        enemyRb.isKinematic = false;

        forceAngle = enemyData.angle;
        CalcForceDirection();
    }

    virtual protected void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Attack(col);
        }
        if (col.gameObject.CompareTag("Stage"))
        {
            num--;
            if (num == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    //攻撃
    void Attack(Collision2D col)
    {
        col.gameObject.GetComponent<PlayerController>().KnockBack(1, this.transform.position, 15 * enemyData.knockBackValue);
        //col.gameObject.GetComponent<PlayerController>().Damage(enemyData.p);
    }

    public void Damage(float power)
    {
        hp -= power;
        ComboParam.Instance.ResetTime();
        Debug.Log("残り体力：" + hp);
        if (hp <= 0)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            _Destroy();
        }
    }

    private void EnemyMove()
    {
        
    }

    void _Destroy()
    {
        //反射用のコライダーに変更
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<CircleCollider2D>().enabled = true;
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.constraints = RigidbodyConstraints2D.None;
        
        //吹っ飛び開始
        BoostSphere();
    }

    void BoostSphere()
    {
        // 向きと力の計算
        Vector2 force = speed * forceDirection;

        // 力を加えるメソッド
        enemyRb.velocity = force;
    }

    void CalcForceDirection()
    {
        // 入力された角度をラジアンに変換
        float rad = forceAngle * Mathf.Deg2Rad;

        // それぞれの軸の成分を計算
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        // Vector3型に格納
        forceDirection = new Vector2(x, y);
    }
}
