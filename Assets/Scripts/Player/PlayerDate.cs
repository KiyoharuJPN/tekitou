using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
    [System.Serializable]
    struct PlayerDatas
    {
        internal PlayerSE playerSE;
        internal Player_Jump jump;

        struct PlayerEffects
        {
            internal GameObject RunEffect;

            internal GameObject JumpEffect;

            internal GameObject heelEffect;

            internal GameObject ExAttackHitEffect;

            internal GameObject ExAttackLastEffect;
        }

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
        }

        [SerializeField]
        [Header("移動ステータス")]
        internal MoveData moveData;

        [SerializeField]
        [Header("ジャンプステータス")]
        internal JumpData jumpData;

        [SerializeField]
        [Header("ノックバックステータス")]
        internal KnockBackData knockBack;

        //背景
        [SerializeField]
        internal ParallaxBackground parallaxBackground;

        //通常攻撃再使用確認
        internal bool isNomalAttack;
        internal bool canNomalAttack;

        //技関係Bool関連（is:その技中か　can:その技が使用可能か）
        internal bool isUpAttack;
        internal bool canUpAttack;
        internal bool isDropAttack;
        internal bool canDropAttack;
        internal bool isSideAttack;
        internal bool canSideAttack;
        internal bool isExAttack;
        internal bool canExAttack;

        //KnockBack関連
        internal Vector2 knockBackDir;   //ノックバックされる方向
        internal bool isKnockingBack;    //ノックバックされているかどうか
        internal float knockBackCounter; //時間を測るカウンター
        internal float canMovingCounter;
        internal float knockBackForce;   //ノックバックされる力
        [SerializeField, Header("HPゲージ")]
        internal HPparam hpparam;

        //入力キー
        internal bool isAttack;
        internal bool isAttackKay;
        internal bool isSkillAttackKay;

        //攻撃時・必殺技時に使用する為のEnemyList
        [SerializeField, Header("exAttackArea")]
        private ExAttackArea exAttacArea;
        internal List<GameObject> enemylist;
        internal List<GameObject> exAttackEnemylist;

        //ダメージカメラ処理
        [SerializeField]
        internal CameraShake shake;

        //アニメーション用
        internal bool isFalling;
        internal bool isMoving;
        internal bool isRun;
        internal bool isJumping;
        internal bool isLanding;
        internal bool isSquatting;
        internal bool isWarpDoor;
        internal int attckType;
        internal bool isGround;
        internal float animSpeed;

        //無敵時間
        internal bool inInvincibleTimeKnockBack, inInvincibleTimeLife;
        public float InvincibleTime;
        SpriteRenderer sprite;

        //boss判定用
        internal bool canMove;

        //死亡判定
        bool isDead;
        public bool GetIsDead { get { return isDead; } }
    }
}
