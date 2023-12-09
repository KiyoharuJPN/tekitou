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
            [Tooltip("�������x")]
            public float firstSpeed;
            [Tooltip("�W�����v���ړ����x")]
            public float jumpFirstSpeed;
            [Tooltip("�_�b�V���ω����x")]
            public float dashSpeed;
            [Tooltip("�ō����x")]
            public float maxSpeed;
            [Tooltip("�����x")]
            public float accele;
            [Tooltip("�����K�v����")]
            public float acceleTime;
        }

        [System.Serializable]
        internal struct JumpData
        {
            [Tooltip("���x")]
            public float speed;
            [Tooltip("�������x")]
            public float gravity;
            [Tooltip("�W�����v�\����")]
            public float jumpHeight;
            [Tooltip("2�i�ڃW�����v�\����")]
            public float secondJumpHeight;
            [Tooltip("�W�����v���Ԃ̏��")]
            public float maxJumpTime;
        }

        [System.Serializable]
        internal struct KnockBackData
        {
            [Tooltip("KnockBack�������Ԏw��")]
            public float knockBackTime;
            [Tooltip("�s���s�\����")]
            public float cantMovingTime;
            [Tooltip("KnockBack�\���ǂ���")]
            public bool canKnockBack;
        }

        [SerializeField]
        [Header("�ړ��X�e�[�^�X")]
        internal MoveData moveData;

        [SerializeField]
        [Header("�W�����v�X�e�[�^�X")]
        internal JumpData jumpData;

        [SerializeField]
        [Header("�m�b�N�o�b�N�X�e�[�^�X")]
        internal KnockBackData knockBack;

        //�w�i
        [SerializeField]
        internal ParallaxBackground parallaxBackground;

        //�ʏ�U���Ďg�p�m�F
        internal bool isNomalAttack;
        internal bool canNomalAttack;

        //�Z�֌WBool�֘A�iis:���̋Z�����@can:���̋Z���g�p�\���j
        internal bool isUpAttack;
        internal bool canUpAttack;
        internal bool isDropAttack;
        internal bool canDropAttack;
        internal bool isSideAttack;
        internal bool canSideAttack;
        internal bool isExAttack;
        internal bool canExAttack;

        //KnockBack�֘A
        internal Vector2 knockBackDir;   //�m�b�N�o�b�N��������
        internal bool isKnockingBack;    //�m�b�N�o�b�N����Ă��邩�ǂ���
        internal float knockBackCounter; //���Ԃ𑪂�J�E���^�[
        internal float canMovingCounter;
        internal float knockBackForce;   //�m�b�N�o�b�N������
        [SerializeField, Header("HP�Q�[�W")]
        internal HPparam hpparam;

        //���̓L�[
        internal bool isAttack;
        internal bool isAttackKay;
        internal bool isSkillAttackKay;

        //�U�����E�K�E�Z���Ɏg�p����ׂ�EnemyList
        [SerializeField, Header("exAttackArea")]
        private ExAttackArea exAttacArea;
        internal List<GameObject> enemylist;
        internal List<GameObject> exAttackEnemylist;

        //�_���[�W�J��������
        [SerializeField]
        internal CameraShake shake;

        //�A�j���[�V�����p
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

        //���G����
        internal bool inInvincibleTimeKnockBack, inInvincibleTimeLife;
        public float InvincibleTime;
        SpriteRenderer sprite;

        //boss����p
        internal bool canMove;

        //���S����
        bool isDead;
        public bool GetIsDead { get { return isDead; } }
    }
}
