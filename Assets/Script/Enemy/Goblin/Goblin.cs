using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    Animator animator;

    public float movingSpeed;
    public float idleTime = 2.4f;

    float x = 1,y = 1;

    bool IsBlowing = false, IsMoving = true, IsAttacking = false, AttackChecking = true;
    float moveSpeed;

    protected override void Start()
    {
        moveSpeed = movingSpeed * -1;
        animator = GetComponent<Animator>();
        base.Start();
    }
    protected override void Update()
    {
        //âÊñ ì‡Ç…Ç†ÇÈ
        if (OnCamera)
        {
            //îÚÇŒÇ≥ÇÍÇƒÇ»Ç¢å¿ÇË
            if (!isDestroy)
            {
                Movement();
            }
            
        }

        animator.SetBool("IsAttacking", IsAttacking);
        animator.SetBool("IsMoving", IsMoving);
        animator.SetBool("IsBlowing", IsBlowing);
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        base.Update();
    }

    //ÉSÉuÉäÉìÇÃìÆÇ´
    void Movement()
    {
        //äRîªíf
        if (!Physics.Linecast(new Vector2(transform.position.x - x + 0.01f, transform.position.y - y + 0.01f), new Vector2(transform.position.x - x, transform.position.y - y))) Debug.Log("1111111111111111111111111111111111111111111111111111"); //moveSpeed *= -1;
        Debug.Log(new Vector2(transform.position.x - x + 0.01f, transform.position.y - y + 0.01f));
        Debug.Log("\n"+new Vector2(transform.position.x - x, transform.position.y - y));

        //à⁄ìÆ
        if (IsMoving)
        {
            enemyRb.velocity = new Vector2(moveSpeed, enemyRb.velocity.y);
        }
    }

    IEnumerator Attacking()
    {
        //ÉAÉjÉÅí≤êÆ
        IsMoving = false;
        IsAttacking = true;

        //çUåÇÇ∑ÇÈÇ∆Ç´ÇÃìÆÇ´
        int i = 0;
        while (i < 10)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        while (i < 15)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        while (i < 20)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        while (i < 21)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }
        while (i < 30)
        {
            i++;
            yield return new WaitForSeconds(0.01f);
        }

        //ÉAÉjÉÅí≤êÆ
        IsAttacking = false;
        StartCoroutine(Idling());
    }

    IEnumerator Idling()
    {
        yield return new WaitForSeconds(idleTime);
        IsMoving = true;
        AttackChecking = true;
    }

    public void PlayerInAttackArea()
    {
        if (IsMoving&&AttackChecking)
        {
            AttackChecking = false;
            StartCoroutine(Attacking());
        }
    }
}
