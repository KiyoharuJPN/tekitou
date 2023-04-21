using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Item;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    string id;
    EnemyData enemyData;

    float hp;

    enum moveType
    {
        NotMove, //�����Ȃ�
        Move,    //����
        FlyMove�@//���
    }
    moveType type;

    private void Start()
    {
        //id�Ŏw�肵���G�f�[�^�Ǎ�
        enemyData = EnemyGeneratar.instance.EnemySet(id);
        hp = enemyData.hp;
    }

    virtual protected void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Attack(col);
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
        Debug.Log(enemyData.charName + "�c��̗́F" + hp);
        if (hp <= 0)
        {
            PointParam.Instance.SetPoint(PointParam.Instance.GetPoint() + enemyData.score);
            Destroy(gameObject);
        }
    }

    private void EnemyMove()
    {
        
    }
}
