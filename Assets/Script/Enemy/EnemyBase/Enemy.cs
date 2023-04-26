using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

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
    //反射回数
    int num;

    GameObject player;
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
        num = enemyData.num;
        forceAngle = enemyData.angle;
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
        CalcForceDirection();
        //吹っ飛び開始
        BoostSphere();
        gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
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

        //オブジェクトを取得
        player = serchTag(gameObject, "Player");

        // それぞれの軸の成分を計算
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        //プレイヤーと自身の位置関係を調査
        if(player.transform.position.y + 1f < this.transform.position.y) 
        { y = -y; }
        if(player.transform.position.x > this.transform.position.x) 
        { x = -x; }
        
        // Vector3型に格納
        forceDirection = new Vector2(x, y);
    }

    //指定されたタグの中で最も近いものを取得
    GameObject serchTag(GameObject nowObj, string tagName)
    {
        float tmpDis = 0;           //距離用一時変数
        float nearDis = 0;          //最も近いオブジェクトの距離
        //string nearObjName = "";    //オブジェクト名称
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            //一時変数に距離を格納
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                //nearObjName = obs.name;
                targetObj = obs;
            }

        }
        //最も近かったオブジェクトを返す
        //return GameObject.Find(nearObjName);
        return targetObj;
    }
}
