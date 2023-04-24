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

    //������ъp�x
    float forceAngle;
    Vector2 forceDirection = new Vector3(1.0f, 1.0f);

    [SerializeField]
    [Header("������ё��x")]
    float speed;
    [SerializeField]
    [Header("���ˉ�")]
    int num;

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
        Debug.Log("�c��̗́F" + hp);
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
        
        //������ъJ�n
        BoostSphere();
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

        // ���ꂼ��̎��̐������v�Z
        float x = Mathf.Cos(rad);
        float y = Mathf.Sin(rad);

        // Vector3�^�Ɋi�[
        forceDirection = new Vector2(x, y);
    }
}
