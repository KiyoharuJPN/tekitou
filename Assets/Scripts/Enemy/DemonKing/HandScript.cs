using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : Enemy
{
    DemonKing demonKing;
    Renderer spritehand;
    public bool isLeftHand;

    protected override void Start()
    {
        demonKing = GetComponentInParent<DemonKing>();
        //敵の点滅
        spritehand = GetComponent<Renderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerEnter2D(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(demonKing != null)
        {
            demonKing.OnHandTriggerStay2D(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (demonKing != null)
        {
            demonKing.OnHandTriggerExit2D(collision);
        }
    }
    
    //動き関連
    void StopHandAnimation()
    {
        GetComponent<Animator>().speed = 0;
    }
    void ResetHandOffect()
    {
        Vector2 posos = GetResetOffsetByPattern(demonKing.GetAnimCtrlPreb());
        transform.position = new Vector2(transform.position.x + posos.x, transform.position.y + posos.y);
    }
    Vector2 GetResetOffsetByPattern(int pt)
    {
        Vector2 offset = Vector2.zero;
        if(isLeftHand)
        {
            switch(pt)
            {
                case -1:
                case 0:
                case 1:
                    break;
                case 2:
                    offset.y = -2.55f;
                    break;
                case 3:
                    offset.x = 3.1f;
                    break;
                default:
                    Debug.Log("有り得ないパターンが読み込まれました、もう一度チェックしてください");
                    break;
            }
        }
        else
        {
            switch (pt)
            {
                case -1:
                case 0:
                    break;
                case 1:
                    offset.y = -2.55f;
                    break;
                case 2:
                    break;
                case 3:
                    offset.x = -3.1f;
                    break;
                default:
                    Debug.Log("有り得ないパターンが読み込まれました、もう一度チェックしてください");
                    break;
            }
        }
        return offset;
    }
    void SetSummonOffset()
    {
        transform.position = new Vector2(transform.position.x, transform.position.y + 2.55f);
    }
    void SetPincerAttackOffset()
    {
        if (demonKing.GetSkillAnimationController() == 0)
        {
            if (isLeftHand) transform.position = new Vector2(transform.position.x - 3.1f, transform.position.y);
            else transform.position = new Vector2(transform.position.x + 3.1f, transform.position.y);
        }
    }

    public void DemonDead()
    {
        //必殺技ヒットエフェクト消す
        BossCheckOnCamera = false;
        OnCamera = false;

        isDestroy = true;
        gameObject.layer = LayerMask.NameToLayer("DeadBoss");
    }

    //ダメージ処理呼出し
    public override void Damage(float power, Skill skill, bool isHitStop, bool exSkill = false)
    {
        if (gameObject.layer == LayerMask.NameToLayer("DeadBoss")) return;
        if (isDestroy) return;
        demonKing.Damage(power, skill, isHitStop, exSkill);
        DamegeProcess(power, skill, isHitStop, exSkill);
    }

    //ダメージ処理（ヒットストップの関係でコルーチンに変更）
    protected override void DamegeProcess(float power, Skill skill, bool isHitStop, bool exSkill)
    {
        //ヒットエフェクト生成
        if (skill != null)
        {
            HitEfect(this.transform, skill.hitEffectAngle);
        }
        else HitEfect(this.transform, UnityEngine.Random.Range(0, 360));


        //ヒット時演出（敵点滅）
        if (!hadDamaged)
        {
            StartCoroutine(HadDamagedHand());
            hadDamaged = true;
        }
    }
    //ヒットエフェクト生成
    override internal void HitEfect(Transform enemy, int angle)
    {
        Vector2 PosOffset = GetResetOffsetByPattern(demonKing.GetAnimCtrlPreb());
        GameObject prefab =
        Instantiate(GameManager.Instance.hitEffect, new Vector2(enemy.position.x + PosOffset.x, enemy.position.y + PosOffset.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0, 0, angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.MonsterGetHit);
        _EfectDestroy(prefab, 0.2f);
    }

    //プレイヤーが攻撃エリアに要る時の動き（AttackCheckAreaから呼ばれる）
    public override bool PlayerInAttackArea()
    {
        var InAttack = false;
        ////trueの修正は各スクリプトで書いてください。
        //if (IsMoving && AttackChecking)
        //{
        //    AttackChecking = false;
        //    //コルーチン関数をここで作ります
        //    InAttack = true;
        //}
        return InAttack;
    }
    //攻撃されたどうかをチェック
    public override bool GetPlayerAttacked()
    {
        //trueの修正は各スクリプトで書いてください。
        return PlayerNotAttacked;
    }
    public override void SetPlayerAttacked(bool PNA)
    {
        PlayerNotAttacked = PNA;
    }

    protected IEnumerator HadDamagedHand()
    {
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        yield return new WaitForSeconds(.05f);
        spritehand.material.color = new Color(1, .3f, .3f);
        yield return new WaitForSeconds(.1f);
        spritehand.material.color = new Color(1, 1, 1);
        hadDamaged = false;
    }

    //技関連
    //敵停止処理
    public override void EnemyStop()
    {
        demonKing.EnemyStop();
        if (enemyRb != null)
        {
            isPlayerExAttack = true;
            enemyRb.velocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.speed = 0;
        }
    }

    //必殺技が当たっていた場合のダメージ処理呼出し
    public override void PlaeyrExAttack_HitEnemyEnd(float power)
    {
        demonKing.PlaeyrExAttack_HitEnemyEnd(power);
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
        Damage(power, null, true, true);
    }

    //停止処理解除
    public override void Stop_End()
    {
        demonKing.Stop_End();
        if (animator != null)
        {
            animator.speed = 1;
        }
        isPlayerExAttack = false;
    }


    //要らない処理を消す
    override protected void OnColEnter2D(Collider2D col) { }

    protected override void OnCollisionEnter2D(Collision2D collision) { }

    override protected void OnColStay2D(Collider2D col) { }

    override protected void Update() { }

    override protected void FixedUpdate() { }

    //死亡時処理
    protected override void OnDestroyMode() { }

    //重力
    protected override void Gravity() { }
}
