using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using PlayerData;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] internal Rigidbody2D rb;
    [SerializeField] internal Animator animator;

    internal PlayerSE playerSE;
    internal Player_Jump jump;

    [SerializeField]
    internal GameObject RunEffect;

    [SerializeField]
    internal GameObject JumpEffect;

    [SerializeField]
    internal GameObject heelEffect;

    [SerializeField]
    internal GameObject ExAttackHitEffect;

    [SerializeField]
    internal GameObject ExAttackLastEffect;

    [System.Serializable]
    public struct MoveData
    {
        [Tooltip("初期速度")]
        public float firstSpeed;
        [Tooltip("ジャンプ中移動速度")]
        public float jumpFirstSpeed;
        [Tooltip("ダッシュ変化速度")]
        public float dashSpeed;
        [Tooltip("最高速度")]
        public float maxSpeed;
        [Tooltip("加速度")]
        public float accele;
        [Tooltip("加速必要時間")]
        public float acceleTime;
    }

    [System.Serializable]
    internal struct JumpData
    {
        [Tooltip("速度")]
        public float speed;
        [Tooltip("落下速度")]
        public float gravity;
        [Tooltip("ジャンプ可能高さ")]
        public float jumpHeight;
        [Tooltip("2段目ジャンプ可能高さ")]
        public float secondJumpHeight;
        [Tooltip("ジャンプ時間の上限")]
        public float maxJumpTime;
    }

    [System.Serializable]
    internal struct KnockBackData
    {
        [Tooltip("KnockBackされる期間指定")]
        public float knockBackTime;
        [Tooltip("行動不能期間")]
        public float cantMovingTime;
        [Tooltip("KnockBack可能かどうか")]
        public bool canKnockBack;
        //[Tooltip("KnockBackされる速さ")]
        //public float knockBackForce;
        //[Tooltip("無敵時間")]
        //public float invincibiltyTime;
    }

    internal enum PlayerState
    {
        Idle,
        NomalAttack,
        UpAttack,
        SideAttack,
        DownAttack,
        ExAttack,
        Event
    }
    
    internal PlayerState playerState;

    [SerializeField]
    [Header("移動ステータス")]
    internal MoveData moveData = new MoveData { firstSpeed = 1f,dashSpeed = 3f, maxSpeed = 10f, accele = 0.03f, acceleTime = 0.3f };

    [SerializeField]
    [Header("ジャンプステータス")]
    internal JumpData jumpData = new() { speed = 16f, gravity = 10f,jumpHeight = 5f, maxJumpTime = 1f };

    [SerializeField]
    [Header("ノックバックステータス")]
    internal KnockBackData knockBack = new() { /*knockBackForce = 50,*/ knockBackTime = .4f, cantMovingTime = .4f, canKnockBack = true };

    //背景
    [SerializeField]
    internal ParallaxBackground parallaxBackground;

    //通常攻撃再使用確認
    internal bool isNomalAttack = false;
    internal bool canNomalAttack = true;

    //技関係Bool関連（can:その技が使用可能か）
    internal bool canUpAttack = true;
    internal bool canDropAttack = true;
    internal bool canSideAttack = true;
    internal bool isExAttack = false;
    internal bool canExAttack = false;

    internal bool isUpAttack = false;
    internal bool isSideAttack = false;

    //KnockBack関連
    internal Vector2 knockBackDir;   //ノックバックされる方向
    internal bool isKnockingBack;    //ノックバックされているかどうか
    internal float knockBackCounter; //時間を測るカウンター
    internal float canMovingCounter;
    internal float knockBackForce;   //ノックバックされる力
    [SerializeField, Header("HPゲージ")]
    internal HPparam hpparam;

    //入力キー
    internal bool isAttack = false;
    internal bool isNomalAttackKay = false;
    internal bool isSkillAttackKay = false;

    //攻撃時・必殺技時に使用する為のEnemyList
    [SerializeField, Header("exAttackArea")]
    internal ExAttackArea exAttacArea;
    internal List<GameObject> enemylist = new List<GameObject>();
    internal List<GameObject> exAttackEnemylist = new List<GameObject>();

    //ダメージカメラ処理
    [SerializeField]
    internal CameraShake shake;

    //アニメーション用
    internal bool isFalling = false;
    internal bool isMoving = false;
    internal bool isRun = false;
    internal bool isJumping = false;
    internal bool isLanding = false;
    internal bool isSquatting = false;
    internal bool isWarpDoor = false;

    internal bool isGround = false;
    internal float animSpeed = 1f;

    //無敵時間
    internal bool inInvincibleTimeKnockBack = false, inInvincibleTimeLife = false;
    public float InvincibleTime = 20;
    SpriteRenderer sprite;

    //boss判定用
    internal bool canMove = true;

    //死亡判定
    bool isDead = false, isgroundpreb = false;
    public bool GetIsDead {  get { return isDead; } }

    //InputSystem
    internal InputAction move, jumpKay, nomalAttack, skillAttack, exAttack_L, exAttack_R;

    void Start()
    {
        playerSE = GetComponent<PlayerSE>();
        rb = GetComponent<Rigidbody2D>();
        jump = GetComponent<Player_Jump>();
        //hpparam = GameObject.Find("UI").GetComponentInChildren<HPparam>();
        animator.SetFloat("Speed", animSpeed);

        sprite = GetComponent<SpriteRenderer>();

        //InputSystem
        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
        jumpKay = playerInput.actions["Jump"];
        nomalAttack = playerInput.actions["NomalAttack"];
        skillAttack = playerInput.actions["SkillAttack"];
        exAttack_L = playerInput.actions["ExAttack_L"];
        exAttack_R = playerInput.actions["ExAttack_R"];
    }

    void Update()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsSquatting", isSquatting);
        animator.SetBool("IsLanding", isLanding);
        animator.SetBool("IsGround", isGround);

        if (playerState == PlayerState.Event)
        {
            rb.velocity = Vector2.zero;
            gameObject.layer = LayerMask.NameToLayer("PlayerAction");
            return;
        }

        if (isGround != isgroundpreb) { isgroundpreb = isGround; Debug.Log("player" + isGround + "and" + isgroundpreb); }
        //Debug.Log(isgroundpreb);

        //ノックバック処理
        if (knockBack.canKnockBack)
        {
            if (isKnockingBack)
            {
                KnockingBack();
                animator.SetBool("IsknockBack", isKnockingBack);
                if (!canNomalAttack)
                {
                    isAttack = false;
                    canNomalAttack = true;
                }
                return;
            }
        };

        if (canMovingCounter >= 0)
        {
            canMovingCounter -= Time.deltaTime;
        }

        AttacKInputKay();

        
    }

    public void Attack(Collider2D enemy, float powar, Skill skill, bool isHitStop)
    {
        ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        ExAttackParam.Instance.AddGauge();
        if (enemy.GetComponent<Enemy>() != null)
        {
            enemy.GetComponent<Enemy>().Damage(powar + ComboParam.Instance.GetPowerUp(), skill, isHitStop);
        }
        else
        {
            enemy.GetComponent<PartsEnemy>().Damage(powar + ComboParam.Instance.GetPowerUp(), skill, isHitStop);
        }
    }

    public void Heel(int resilience)
    {
        hpparam.SetHP(hpparam.GetHP() + resilience);
        HeelEffect();
    }

    public virtual void Damage(int power)
    {

        if (gameObject.GetComponent<InvinciblBuff>()) { return; }
        if (!inInvincibleTimeLife)
        {
            //無敵時間の計算
            inInvincibleTimeLife = true;
            StartCoroutine(InvincibleLife());

            //ライフ計算
            hpparam.DamageHP(hpparam.GetHP() - power);
            shake.Shake(0.2f, 0.8f, true, true);
            if (hpparam.GetHP() <= 0 && !isDead)
            {
                PlayerDead();
            }
        }
    }

    public void PlayerDead()
    {
        if(isDead) { return; }
        playerState = PlayerState.Event;
        isDead = true;
        this.tag = "DeadPlayer";
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        isKnockingBack = false;
        SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
        animator.Play("Death");
        shake.Shake(0.2f, 1f, true, true);
        GameManager.Instance.PlayerDeath();
    }

    //技入力検知
    protected virtual void AttacKInputKay()
    {
        var inputMoveAxis = move.ReadValue<Vector2>();

        if (nomalAttack.IsPressed())
        {
            isNomalAttackKay = true;
        }
        else { isNomalAttackKay = false; }
        if (skillAttack.IsPressed())
        {
            isSkillAttackKay = true;
        }
        else { isSkillAttackKay = false; }

        if (playerState != PlayerState.Idle || playerState == PlayerState.Event) return;

        //上昇攻撃
        if (inputMoveAxis.y >= 0.9 && isSkillAttackKay)
        {
            AttackAction("UpAttack");
        }
        //落下攻撃攻撃
        if (inputMoveAxis.y <= -0.9 && isSkillAttackKay)
        {
            AttackAction("DawnAttack");
        }
        //横移動攻撃
        if (inputMoveAxis.x >= 0.9 && isSkillAttackKay)
        {
            AttackAction("SideAttack_right");
        }
        else if (inputMoveAxis.x <= -0.9 && isSkillAttackKay)
        {
            AttackAction("SideAttack_left");
        }
        //必殺技
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed())
        {
            AttackAction("ExAttack");
        }
        //手動攻撃：攻撃ボタンが押されせたとき
        if (nomalAttack.WasPressedThisFrame())
        {
            //通常攻撃入力
            AttackAction("NomalAttack");
        }
    }

    //アクション実行
    internal void AttackAction(string actionName)
    {
        switch (actionName)
        {
            case "UpAttack":
                if (!canUpAttack) return; 
                playerState = PlayerState.UpAttack;
                canUpAttack = false;
                UpAttack.UpAttackStart(this, jump,this);
                break;

            case "DawnAttack":
                if (!canDropAttack) return;
                playerState = PlayerState.DownAttack;
                canDropAttack = false;
                DropAttack.DropAttackStart(this,this);
                break;

            case "SideAttack_right":
                if (!canSideAttack) return;
                playerState = PlayerState.SideAttack;
                canSideAttack = false;
                SideAttack.SideAttackStart(this, true, this);
                break;

            case "SideAttack_left":
                if (!canSideAttack) return;
                playerState = PlayerState.SideAttack;
                canSideAttack = false;
                SideAttack.SideAttackStart(this, false, this);
                break;

            case "ExAttack":
                if(!canExAttack) return;
                playerState = PlayerState.ExAttack;
                canExAttack = false;
                ExAttack.ExAttackStart(this);
                break;

            case "NomalAttack":
                playerState = PlayerState.NomalAttack;
                NomalAttack.NomalAttackStart(this,this);
                break;
        }
    }

    //攻撃終了時の処理
    internal async void AttackEnd()
    {
        switch (playerState)
        {
            case PlayerState.Event: break;
            case PlayerState.ExAttack:
                animator.SetBool("IsExAttack", false);
                exAttackEnemylist.Clear();
                break;
            case PlayerState.SideAttack:
                animator.SetBool("IsSideAttack", false);
                break;
            case PlayerState.UpAttack:
                animator.SetBool("IsUpAttack", false);
                break;
            case PlayerState.DownAttack:
                animator.Play("Hero_DropAttack_End");
                animator.SetBool("IsDropAttack", false);
                await UniTask.Delay(270);
                break;
        }
        if(playerState != PlayerState.Event) 
        {
            Debug.Log("アイドルに移行");
            playerState = PlayerState.Idle;
        }
        enemylist.Clear();
    }

    //KnockBackされたときの処理
    void KnockingBack()
    {
        
        if (knockBackCounter == knockBack.knockBackTime)
        {
            rb.velocity = Vector2.zero;
            isAttack = false;
        }
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;
            //簡単に説明すると下は上下に飛ばさないコードです。
            rb.AddForce(new Vector2((knockBackDir.y * knockBackForce > 5 || knockBackDir.y * knockBackForce < -5) ? ((knockBackDir.x < 0) ? -Mathf.Abs(knockBackDir.y * knockBackForce) : Math.Abs(knockBackDir.y * knockBackForce)) : knockBackDir.x * knockBackForce, ((knockBackDir.y * knockBackForce> 5 || knockBackDir.y * knockBackForce < -5)? knockBackDir.y : knockBackDir.y * knockBackForce)));//横だけ飛ばされるコード      簡単に説明すると上と下は５よりでかくなると飛ばされない、左右に関して上下が５以上になると百パーセント横から触ったということじゃないのが分かるので、上下の飛ばす力で左右の方向を与えて飛ばさせる。
        }
        else
        {
            isKnockingBack = false;
        }
    }

    //KnockBackされたら呼ぶ関数
    public void KnockBack(Vector3 position, float force)
    {
        if (!inInvincibleTimeKnockBack)
        {
            // 無敵時間の計算
            inInvincibleTimeKnockBack = true;
            StartCoroutine(InvincibleKnockBack());

            //ノックバック
            canMovingCounter = knockBack.cantMovingTime;
            knockBackCounter = knockBack.knockBackTime;
            isKnockingBack = true;

            knockBackDir = transform.position - position;
            knockBackDir.Normalize();
            knockBackForce = force;
        }
    }

    //必殺技
    internal void CanExAttackCheck() //必殺技使用可能チェック
    {
        if (ExAttackParam.Instance.GetCanExAttack)
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.exGageMax);
            canExAttack = true;
        }
    }

    public void ExAttackStart()
    {
        animator.SetTrigger("ExAttack");
    }

    //---------------------------------------
    //必殺技処理（アニメーションから呼ぶ）
    public void _ExAttackHitEffect()
    {
        //エフェクト生成
        foreach (var enemy in exAttackEnemylist)
        {
            if (enemy == null) return;
            HitEfect(enemy.transform, UnityEngine.Random.Range(0, 360));
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
    }

    public void _ExAttackHitEnemyDamage()
    {
        //ダメージ処理
        Skill skill = SkillGenerater.instance.SkillSet(Skill.Type.ExAttack);
        
        GameManager.Instance.PlayerExAttack_HitEnemyEnd(exAttackEnemylist, skill.damage + ComboParam.Instance.GetPowerUp());
    }
    public void ExAttackHitCheck()
    {
        if(exAttackEnemylist.Count == 0)
        {
            ExAttackEnd();
        }
    }
    //必殺技で使用したEnemyをリセット
    public void ExAttackEnd()
    {
        exAttackEnemylist.Clear();
        NomalPlayer();
        GameManager.Instance.PlayerExAttack_End();
        AttackEnd();
    }
    //---------------------------------------

    //スキルアクション中無敵に使用するメソッド
    public void SkillActionPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
    }
    //無敵をはがすためのメソッド
    public void NomalPlayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    //スキルアクション終了メソッド
    public void SkillAttackEnd()
    {
        isAttack = false;
        enemylist.Clear();
    }

    //イベント開始
    public void EventStart()
    {
        playerState = PlayerState.Event;
    }
    //イベント終了
    public void EventEnd()
    {
        playerState = PlayerState.Idle;
    }

    //砂埃エフェクト生成
    public void _RunEffect()
    {
        GameObject prefab = 
        Instantiate(RunEffect, new Vector2(this.transform.position.x, this.transform.position.y - 0.8f), Quaternion.identity);
        if(gameObject.transform.localScale.x < 0)
        {
            Vector2 scale = prefab.transform.localScale;
            scale.x *= -1f;
            prefab.transform.localScale = scale;
        }
        _EfectDestroy(prefab, 0.3f);
    }

    //ダブルジャンプエフェクト生成
    public void _JumpEffect()
    {
        GameObject prefab =
        Instantiate(JumpEffect, new Vector2(this.transform.position.x, this.transform.position.y - 0.8f), Quaternion.identity);
        if (gameObject.transform.localScale.x < 0)
        {
            Vector2 scale = prefab.transform.localScale;
            scale.x *= -1f;
            prefab.transform.localScale = scale;
        }
        _EfectDestroy(prefab, 0.2f);
    }

    internal void HitEfect(Transform enemy, int angle)
    {
        GameObject prefab =
        Instantiate(ExAttackHitEffect, new Vector2(enemy.position.x, enemy.position.y), Quaternion.identity);
        prefab.transform.Rotate(new Vector3(0,0,angle));
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_Hit);
        _EfectDestroy(prefab, 0.2f);
    }

    //ヒールエフェクト生成
    void HeelEffect()
    {
        GameObject prefab =
        Instantiate(heelEffect, new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity);
        prefab.transform.parent = this.transform.transform;
        _EfectDestroy(prefab, 1f);
    }

    public void _ExAttackLastEffect()
    {
        GameObject prefab;
        if (gameObject.transform.localScale.x < 0)
        {
            prefab =
            Instantiate(ExAttackLastEffect, new Vector2(this.transform.position.x - 4f, this.transform.position.y), Quaternion.identity);
        }
        else
        {
            prefab =
            Instantiate(ExAttackLastEffect, new Vector2(this.transform.position.x + 4f, this.transform.position.y), Quaternion.identity);
        }
        _EfectDestroy(prefab, 0.3f);
    }

    void _EfectDestroy(GameObject prefab, float time)
    {
        Destroy(prefab, time);
    }

    public void SetCanMove(bool cM)
    {
        rb.velocity = Vector2.zero;
        if(cM)
        {
            playerState = PlayerState.Idle;
        }
        else if(!cM) 
        {
            playerState = PlayerState.Event;
        }
        canMove = cM;
        isUpAttack = false;
        isSideAttack = false;
        isNomalAttack = false;
        canDropAttack = true;
        animator.SetBool("IsDropAttack", false);
        AttackEnd();
    }

    internal void WarpDoor(Transform door)
    {
        isRun = false;
        isMoving = false;
        isJumping = false;
        isFalling = false;
        isSquatting = false;
        animator.SetBool("IsRun", isRun);
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
        animator.SetBool("IsSquatting", isSquatting);
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        transform.localScale = new Vector3(1, 1, 1);
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        this.rb.velocity = new Vector2 (0, 0);
        animator.SetBool("IsWarpDoor", true);
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
        animator.SetTrigger("WarpDoor");
    }

    internal void WarpDoorEnd()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        animator.SetBool("IsWarpDoor", false);
    }

    internal void GoolDoor(Transform door)
    {
        isWarpDoor = true;
        var doorPosX = door.position.x;
        var doorPosY = door.position.y;
        this.transform.position = new Vector3(doorPosX, doorPosY, transform.position.z);
        this.rb.velocity = new Vector2(0, 0);
        animator.SetTrigger("GoolDoor");
        SoundManager.Instance.PlaySE(SESoundData.SE.GoalSE);
    }

    //背景スクロール処理
    private void BackgroundScroll()
    {
        if (parallaxBackground != null)
        {
            parallaxBackground.StartScroll(this.transform.position);
        }
    }

    protected IEnumerator InvincibleKnockBack()
    {
        var n = InvincibleTime;
        while (n > 0)
        {
            n--;
            if(n%2 == 0)
            {
                sprite.color = new Color(1, 1, 1);
            }
            else
            {
                sprite.color = new Color(1, .3f, .3f);
            }
            yield return new WaitForSeconds(0.01f);
        }
        inInvincibleTimeKnockBack = false;
    }

    protected IEnumerator InvincibleLife()
    {
        var n = InvincibleTime;
        while (n > 0)
        {
            n--;
            yield return new WaitForSeconds(0.01f);
        }
        inInvincibleTimeLife = false;
    }

    public void AddAttack(SlashingBuff.SlashingType type)
    {
        if (gameObject.GetComponent<SlashingBuff>())
            gameObject.GetComponent<SlashingBuff>().Slashing(type, gameObject);
    }
}
