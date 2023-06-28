using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard_MagicBall : MonoBehaviour
{
    ////���������̖��p�t�̍U����H�炤���Ƃ������悤��
    //[System.NonSerialized]
    //public static bool WizardHadAttack;
    public float WMBClearTime = 10;

    //�U���͊֘A
    float knockBackValue;
    int attackPower;

    //���W�b�h�{�f�B
    Rigidbody2D WMBRb;

    bool clearWMB = true;

    private void Awake()
    {
        WMBRb = GetComponent<Rigidbody2D>();

    }
    private void OnEnable()
    {
        StartCoroutine(WMBClear(WMBClearTime));
        clearWMB = true;
    }

    //�����Ԃ��������̏���
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

    //�U���͂ƃm�b�N�o�b�N�͂̐ݒ�y�ѐi�s�����̏�����
    public void AKForce(int atkpower,float kbvpower,Vector2 direction)
    {
        attackPower = atkpower;
        knockBackValue = kbvpower;
        WMBRb.velocity = direction;
    }

    //�U�����R�[�h
    void Attack(Collider2D col)
    {
        //if (!WizardHadAttack)
        //{
        //    //�U���N�[���_�E���^�C��
        //    WizardHadAttack = true;
        //    StartCoroutine(WHadAttackReset());
        //�_���[�W�ƃm�b�N�o�b�N
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

    //    //�U���N�[���_�E��
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
}
