using UnityEngine;
using Unity.Collections;
using System.Collections;

public class Devil : Enemy
{
    //Animation�֘A
    float IdleTimer, IdleCounter, IdleFlyPosPreY, IdleFlyingSpeed;
    public float IdleTime = 3.6f;
    [SerializeField, Tooltip("�Î~���Ԃ�Time�ő��邩���[�v�񐔂ő��邩")]
    bool UseLoopToResetTimer = false;
    [SerializeField, Tooltip("���[�v�񐔂ŐÎ~���Ԃ��v�鎞�̂ݎg�p")]
    int Frequency = 3, AnimCtrl = 0;
    Coroutine IdleAnimCorou;


    //��s�ړ��֘A
    public Vector2 EndPosition, MidPosition;
    Vector2 StartPosition;
    public float FlyingSpeed = 5;
    [SerializeField, Tooltip("�Î~����s��")]
    float IdleFlySpeed = 3;


    //�����ϐ�
    double a, b, c;
    bool rightRoute = false, facingRight = false;

    //�f�t�H���g�֐�
    protected override void Start()
    {
        base.Start();
        if(GetComponent<MonsterHouse_Enemy>() != null ) transform.position = new Vector2(transform.position.x, -5);
        IdleFlyingSpeed = IdleFlySpeed * 0.01f;
        Assignment();
        if(StartPosition.x < EndPosition.x)
        {
            moveSpeed = FlyingSpeed * 0.01f;
            rightRoute = facingRight = true;
        }
        else
        {
            moveSpeed = FlyingSpeed * -0.01f;
        }
        //�ړ��񎟊֐��̌v�Z
        FindQuadraticEquation();
        IsMoving = true;
    }

    protected override void Update()
    {
        base.Update();
        //Debug.Log(GetYFromX(transform.position.x));

        animator.SetBool("IsBlowing", isDestroy);
        animator.SetBool("IsMoving", IsMoving);
    }




    //�ړ��p
    void Movement()
    {
        if (!rightRoute)
        {
            if (transform.position.x <= EndPosition.x && !facingRight)
            {
                facingRight = true;
                IsMoving = false;
            }
            if (transform.position.x >= StartPosition.x && facingRight)
            {
                facingRight = false;
                IsMoving = false;
            }
        }
        else
        {
            if (transform.position.x >= EndPosition.x && facingRight)
            {
                facingRight = false;
                IsMoving = false;
            }
            if (transform.position.x <= StartPosition.x && !facingRight)
            {
                facingRight = true;
                IsMoving = false;
            }
        }
    }



    //�O���֐�




    //�����֐�
    void Moving()
    {
        //�ړ�������֐�
        if(AnimCtrl != 0)AnimCtrl = 0;
        Movement();

        float newX = transform.position.x + moveSpeed;
        //Debug.Log(moveSpeed);
        //Debug.Log(transform.position.x);
        //Debug.Log(newX);

        //Debug.Log(newX);
        transform.position = new Vector2 (newX, GetYFromX(newX));
    }
    void NotMoving()
    {
        ////�Î~���鎞�̊֐�
        //if (enemyRb.velocity != Vector2.zero)
        //{
        //    enemyRb.velocity = Vector2.zero;
        //}

        if (!UseLoopToResetTimer)
        {
            
            //�Î~�^�C�}�[
            IdleTimer += Time.deltaTime;
            if (IdleTimer > IdleTime)
            {
                IsMoving = true;
                TurnAround();
                //�Î~�^�C�}�[���Z�b�g
                IdleTimer = 0;
            }
        }
        else
        {
            //�Î~�J�E���^�[�v�Z
            if (IdleCounter >= Frequency)
            {
                IsMoving = true;
                TurnAround();
                //�Î~�J�E���^�[���Z�b�g
                IdleCounter = 0;
            }
        }

    }
    //Animation�֘A
    protected void IdleAnimFinishLoop()
    {
        if (UseLoopToResetTimer)
            IdleCounter++;
    }
    protected void MovingAnimation()
    {
        var inAnim = false;
        //if (AnimCtrl == 0 && !inAnim)
        //{
        //    IdleFlyPosPreY = transform.position.y;
        //    AnimCtrl++;
        //    Debug.Log("in 1");
        //    inAnim = true;
        //}
        //if (AnimCtrl == 1 && !inAnim)
        //{
        //    transform.position = new Vector2(transform.position.x, transform.position.y + IdleFlyingSpeed);
        //    AnimCtrl++;
        //    Debug.Log("in 2");
        //    inAnim = true;
        //}
        //if (AnimCtrl == 2 && !inAnim)
        //{
        //    transform.position = new Vector2(transform.position.x, transform.position.y + IdleFlyingSpeed);
        //    AnimCtrl++;
        //    Debug.Log("in 3");
        //    inAnim = true;
        //}
        //if (AnimCtrl == 3 && !inAnim)
        //{
        //    transform.position = new Vector2(transform.position.x, transform.position.y - IdleFlyingSpeed);
        //    AnimCtrl++;
        //    Debug.Log("in 4");
        //    inAnim = true;
        //}
        //if(AnimCtrl == 4 && !inAnim)
        //{
        //    transform.position = new Vector2(transform.position.x, IdleFlyPosPreY);
        //    AnimCtrl = 0;
        //    Debug.Log("in 5");
        //    inAnim = true;
        //}


        if (AnimCtrl == 0 && !inAnim)
        {
            inAnim = true;
            IdleFlyPosPreY = transform.position.y;
            if (IdleAnimCorou != null)
                StopCoroutine(IdleAnimCorou);
            IdleAnimCorou = StartCoroutine(FlyUp());
            AnimCtrl = 1;
        }
        if (AnimCtrl == 1 && !inAnim)
        {
            inAnim = true;
            if (IdleAnimCorou != null)
                StopCoroutine(IdleAnimCorou);
            IdleAnimCorou = StartCoroutine(FlyDown());
            AnimCtrl = 2;
        }
        if (AnimCtrl == 2 && !inAnim)
        {
            if (IdleAnimCorou != null)
                StopCoroutine(IdleAnimCorou);
            transform.position = new Vector2(transform.position.x, IdleFlyPosPreY);
            AnimCtrl = 0;
        }
    }
    protected override void FixedUpdate()
    {
        if (isPlayerExAttack) return;
        if (isDestroy) return;

        //�ړ����Ă���Ƃ�
        if (IsMoving)
            Moving();
        //�ړ����Ă��Ȃ��Ƃ�
        if (!IsMoving)
            NotMoving();
    }

    //�R���[�`���֐�
    IEnumerator FlyUp()
    {
        while (true)
        {
            if (!isPlayerExAttack) transform.position = new Vector2(transform.position.x, transform.position.y + IdleFlyingSpeed);
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator FlyDown()
    {
        while (true)
        {
            if (!isPlayerExAttack) transform.position = new Vector2(transform.position.x, transform.position.y - IdleFlyingSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    //�񎟊֐����
    void Assignment()
    {
        StartPosition = transform.position;
        if (MidPosition == Vector2.zero)
            MidPosition = new Vector2(transform.position.x - 2.5f, transform.position.y - 5f);
        if (EndPosition == Vector2.zero)
            EndPosition = new Vector2(transform.position.x - 5f, transform.position.y);
        //Debug.Log(StartPosition);
        //Debug.Log(MidPosition);
        //Debug.Log(EndPosition);
    }
    //�񎟊֐��̌v�Z
    void FindQuadraticEquation()
    {
        float x1 = StartPosition.x, y1 = StartPosition.y, x2 = MidPosition.x, y2 = MidPosition.y, x3 = EndPosition.x, y3 = EndPosition.y;
        double denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
        a = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
        b = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
        c = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;
    }
    //�ʒu�̌v�Z
    float GetYFromX(float x)
    {
        return (float)(a * Mathf.Pow(x, 2) + b * x + c);
    }
}
