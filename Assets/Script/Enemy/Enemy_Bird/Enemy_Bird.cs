using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bird : Enemy
{
    [Tooltip("�ړ����x")]
    public float movingSpeed = 0.5f;                //�ړ����x
    [Header("�ړ��͈�")]
    public float moveArea = 10f;                    //�ړ��͈�
    [Tooltip("true�͍��Ɉړ��Afalse�͉E�Ɉړ�")]
    public bool LRMove;                             //�ړ�����
    [Tooltip("�҂����Ԃ̐ݒ�")]
    public float idleTime = 2.4f;
    [SerializeField]
    [Tooltip("���U��")]
    public GameObject BirdAttack, BirdAttackArea;
    [SerializeField]
    [Tooltip("2D�{�b�N�X�R���C�_�[")]
    public BoxCollider2D monster_upper, monster_lower;


    //�����֐�
    bool AttackChecking = true, PlayerNotAttacked = true;
    Vector2 MovingArea;

    protected override void Start()
    {
        MovingArea = gameObject.transform.position;
        moveSpeed = movingSpeed * -1;
        base.Start();
    }

    protected override void Update()
    {
        if (OnCamera)      //��ʓ��ɂ���Ƃ�
        {
            Movement();
        }


        //�A�j���[�^�[�̐ݒ�
        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        //��΂���Ă��邩�ǂ����̔���
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //Enemy�̌p��
        base.Update();
    }

    void Movement()
    {
        //�ړ��͈͊O�Ɉړ������������ς���
        if(transform.position.x >= MovingArea.x + moveArea / 2 && LRMove)
        {
            TurnAround();
        }else if (transform.position.x <= MovingArea.x - moveArea / 2 && !LRMove)
        {
            TurnAround();
        }

        //�ړ�
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    IEnumerator Attacking()
    {
        //�A�j������
        IsAttacking = true;
        IsMoving = false;
        //�R���C�_�[����
        AttackAnim();

        //�U������Ƃ��̓���
        int i = 0;
        while (i < 7)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        SoundManager.Instance.PlaySE(SESoundData.SE.BirdChirping);

        monster_upper.offset = new Vector2(0f, -0.3f);
        monster_upper.size = new Vector2(3.45f, 2.55f);
        //monster_upper.offset = new Vector2(0f, 0.35f);
        //monster_upper.size = new Vector2(3.1f, 1.3f);
        //monster_lower.offset = new Vector2(-0.3f, -0.5f);
        //monster_lower.size = new Vector2(1.42f, 1.64f);
        BirdAttack.SetActive(true);
        BirdAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-0.85f, -0.65f);
        BirdAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.5f, 1.4f);
        while (i < 14)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        //monster_upper.offset = new Vector2(-0.05f, 0.3f);
        //monster_upper.size = new Vector2(3f, 1.95f);
        //monster_lower.enabled = false;
        BirdAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-0.75f, -0.9f);
        BirdAttack.GetComponent<BoxCollider2D>().size = new Vector2(2f, 1.6f);
        while (i < 21)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        //monster_upper.offset = new Vector2(-0.05f, -0.3f);
        //monster_upper.size = new Vector2(3f, 1.95f);
        BirdAttack.GetComponent<BoxCollider2D>().offset = new Vector2(-0.7f, -1.15f);
        BirdAttack.GetComponent<BoxCollider2D>().size = new Vector2(1.9f, 1.11f);
        while (i < 28)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        BirdAttack.SetActive(false);
        //monster_upper.offset = new Vector2(0f, -0.32f);
        //monster_upper.size = new Vector2(3.3f, 1.26f);
        //monster_lower.enabled = true;
        //monster_lower.offset = new Vector2(-0.4f, -0.54f);
        //monster_lower.size = new Vector2(1.04f, 1.72f);
        while (i < 35)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        monster_upper.offset = new Vector2(0f, 0.08f);
        monster_upper.size = new Vector2(3.45f, 3.4f);
        //monster_upper.offset = new Vector2(0f, 0.65f);
        //monster_upper.size = new Vector2(3.24f, 1.93f);
        //monster_lower.offset = new Vector2(-0.35f, -0.5f);
        //monster_lower.size = new Vector2(0.94f, 1.64f);
        while (i < 42)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        monster_upper.offset = new Vector2(0f, -0.3f);
        monster_upper.size = new Vector2(3.45f, 2.55f);
        //monster_upper.offset = new Vector2(0f, 0.22f);
        //monster_upper.size = new Vector2(3.05f, 1.24f);
        //monster_lower.offset = new Vector2(-0.33f, -0.53f);
        //monster_lower.size = new Vector2(0.90f, 1.69f);
        while (i < 49)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        //monster_upper.offset = new Vector2(-0.08f, -0.3f);
        //monster_upper.size = new Vector2(3f, 1.8f);
        //monster_lower.offset = new Vector2(-0.33f, -0.83f);
        //monster_lower.size = new Vector2(0.90f, 1.58f);
        while (i < 56)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        //monster_upper.offset = new Vector2(-0.08f, -0.38f);
        //monster_upper.size = new Vector2(3f, 1.82f);
        //monster_lower.offset = new Vector2(-0.33f, -0.83f);
        //monster_lower.size = new Vector2(0.90f, 1.58f);
        while (i < 63)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        //monster_upper.offset = new Vector2(0.02f, 0.26f);
        //monster_upper.size = new Vector2(3.19f, 1.1f);
        //monster_lower.offset = new Vector2(-0.38f, -0.71f);
        //monster_lower.size = new Vector2(1f, 1.35f);
        AttackChecking = true;
        BirdAttackArea.SetActive(true);
        while (i < 70)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        IsAttacking = false;
        IsMoving = true;
        WalkAnim();
    }


    //�A�j���[�V�������Ƃ̃R���C�_�[����
    void WalkAnim()
    {
        //���̃R���C�_�[���폜
        //monster_lower.enabled = false;
        //�㕔�̃R���C�_�[���Ĕz�u
        monster_upper.offset = new Vector2(0f, -0.4f);
        monster_upper.size = new Vector2(3.45f, 2.44f);
    }
    void AttackAnim()
    {
        //�㕔�̃R���C�_�[���Ĕz�u
        monster_upper.offset = new Vector2(0f,0.08f);
        monster_upper.size = new Vector2(3.45f, 3.4f);
        ////���̃R���C�_�[�������������čĔz�u
        //monster_lower.offset = new Vector2(-.3f, -.5f);
        //monster_lower.size = new Vector2(1.42f, 1.6f);
        //monster_lower.enabled = true;
    }

    public override void TurnAround()
    {
        LRMove = !LRMove;
        base.TurnAround();
    }

    public bool PlayerInAttackArea()
    {
        var InAttack = false;
        if (IsMoving && AttackChecking && !HadAttack)
        {
            //�ڐG�_���[�W����������Ȃ��ɂ���
            HadAttack = true;
            StartCoroutine(HadAttackReset());
            //���U��
            AttackChecking = false;
            StartCoroutine(Attacking());
            InAttack = true;
        }
        return InAttack;
    }

    public bool GetPlayerAttacked()
    {
        return PlayerNotAttacked;
    }
    public void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }


    /*    //��΂����Ƃ��ɃR���C�_�[��ς���Ƃ��̑���
        protected override void _Destroy()
        {
            //���˗p�̃R���C�_�[�ɕύX
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.GetComponent<CircleCollider2D>().enabled = true;
            enemyRb.bodyType = RigidbodyType2D.Dynamic;
            enemyRb.constraints = RigidbodyConstraints2D.None;
            CalcForceDirection();
            //������ъJ�n
            BoostSphere();
            isDestroy = true;
            gameObject.layer = LayerMask.NameToLayer("PinBallEnemy");
        }*/

}
