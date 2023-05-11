using System.Dynamic;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class Slime : Enemy
{
    [Header("à⁄ìÆÇ∑ÇÈéûÇÃçÇÇ≥Ç∆ãóó£")]
    public float moveHeight, moveWidth;

    Animator animator;
    float movingHeight, movingWidth;
    bool IsBlowing = false, IsMoving = false, moveHideFlag = false, LRmove = false;

    //float TestTime = 0f;
    override protected void Start()
    {
        movingWidth = moveWidth * -1;
        movingHeight = moveHeight;
        animator = GetComponent<Animator>();
        base.Start();
    }
    override protected void Update()
    {
        //âÊñ ì‡Ç…Ç†ÇÈ
        if (OnCamera)
        {
            //îÚÇŒÇ≥ÇÍÇƒÇ»Ç¢å¿ÇË
            if (!IsBlowing) SlimeMove();
        }
        //TestTime += Time.deltaTime;
        //Debug.Log("moving:" + IsMoving + "\nsecond:" + TestTime);

        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        base.Update();
    }

    void SlimeMove()
    {
        //íÖínåvéZ


        //îΩì]èàóù
        if (IsMoving)
        {
            if (enemyRb.velocity.x == 0 && LRmove)
            {
                Vector2 scale = this.transform.localScale;
                scale.x *= -1;
                this.transform.localScale = scale;
                movingWidth *= -1;
                LRmove = false;
            }
        }

        //àÍïbÇÃë“Çøèàóù
        if (IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveFalse());
            //à⁄ìÆèàóùÅB
            enemyRb.AddForce(new Vector2(movingWidth, movingHeight), ForceMode2D.Impulse);
            LRmove = true;
            StartCoroutine(Landing());
        }
        if (!IsMoving && !moveHideFlag)
        {
            moveHideFlag = true;
            StartCoroutine(SetMoveTrue());
        }

        
    }
    IEnumerator Landing()
    {
        yield return new WaitForSeconds(0.5f);
        LRmove = false;
    }
    IEnumerator SetMoveTrue()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = true;
        moveHideFlag = false;
    }
    IEnumerator SetMoveFalse ()
    {
        yield return new WaitForSeconds(1f);
        IsMoving = false;
        moveHideFlag = false;
    }


}
