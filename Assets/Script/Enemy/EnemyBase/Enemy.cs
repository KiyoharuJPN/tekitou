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

    //������ъp�x
    float forceAngle;
    Vector2 forceDirection = new Vector3(1.0f, 1.0f);

    [SerializeField]
    [Header("������ё��x")]
    float speed;
    //���ˉ�
    int num;

    GameObject player;
    enum moveType
    {
        NotMove, //�����Ȃ�
        Move,    //����
        FlyMove�@//���
    }
    moveType type;

    bool isDestroy;

    private void Start()
    {
        //id�Ŏw�肵���G�f�[�^�Ǎ�
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

    //�U��
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
        //���˗p�̃R���C�_�[�ɕύX
        this.GetComponent<BoxCollider2D>().enabled = false;
        this.GetComponent<CircleCollider2D>().enabled = true;
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.constraints = RigidbodyConstraints2D.None;
        CalcForceDirection();
        //������ъJ�n
        BoostSphere();
        gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
    }

    void BoostSphere()
    {
        // �����Ɨ͂̌v�Z
        Vector2 force = speed * forceDirection;

        // �͂������郁�\�b�h
        enemyRb.velocity = force;
    }

    void CalcForceDirection()
    {
        // ���͂��ꂽ�p�x�����W�A���ɕϊ�
        float rad = forceAngle * Mathf.Deg2Rad;

        //�I�u�W�F�N�g���擾
        player = serchTag(gameObject, "Player");

        // ���ꂼ��̎��̐������v�Z
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        //�v���C���[�Ǝ��g�̈ʒu�֌W�𒲍�
        if(player.transform.position.y + 1f < this.transform.position.y) 
        { y = -y; }
        if(player.transform.position.x > this.transform.position.x) 
        { x = -x; }
        
        // Vector3�^�Ɋi�[
        forceDirection = new Vector2(x, y);
    }

    //�w�肳�ꂽ�^�O�̒��ōł��߂����̂��擾
    GameObject serchTag(GameObject nowObj, string tagName)
    {
        float tmpDis = 0;           //�����p�ꎞ�ϐ�
        float nearDis = 0;          //�ł��߂��I�u�W�F�N�g�̋���
        //string nearObjName = "";    //�I�u�W�F�N�g����
        GameObject targetObj = null; //�I�u�W�F�N�g

        //�^�O�w�肳�ꂽ�I�u�W�F�N�g��z��Ŏ擾����
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //���g�Ǝ擾�����I�u�W�F�N�g�̋������擾
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //�I�u�W�F�N�g�̋������߂����A����0�ł���΃I�u�W�F�N�g�����擾
            //�ꎞ�ϐ��ɋ������i�[
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                //nearObjName = obs.name;
                targetObj = obs;
            }

        }
        //�ł��߂������I�u�W�F�N�g��Ԃ�
        //return GameObject.Find(nearObjName);
        return targetObj;
    }
}
