using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyA_Demo : Enemy
{
    private int cmeraReflexNum = 2;

    override protected void Start()
    {
        ///�G��script�Ɋ�Â�
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        Gravity();
    }

    override protected void Update()
    {
        //�A�j���[�^�[�̐ݒ�
        animator.SetBool("IsBlowing", IsBlowing);
        //��Ԃ̕ύX
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //�G��script�Ɋ�Â�
        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage") && isDestroy &&
                collision.gameObject.layer == 20)
        {
            Debug.Log("�J�����G���A����");
            cmeraReflexNum--;
            if (cmeraReflexNum <= 0)
            {
                this.gameObject.layer = 27;
            }
        }
        base.OnCollisionEnter2D(collision);
    }

    protected override void OnColEnter2D(Collider2D col)
    {
        if (!isDestroy && HadContactDamage)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Attack(col);
            }
        }
        if (col.gameObject.CompareTag("Stage") && isDestroy)
        {
            Debug.Log("����");
            if (col.gameObject.layer == 3)
            {
                Debug.Log("�J�����G���A����");
                cmeraReflexNum--;
                if (cmeraReflexNum <= 0)
                {
                    this.gameObject.layer = 27;
                }
            }
            reflexNum--;
            Debug.Log(reflexNum);
            if (reflexNum == 0)
            {
                EnemyNomalDestroy();
            }
        }
    }
}
