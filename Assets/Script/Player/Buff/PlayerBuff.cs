using Unity.VisualScripting;
using UnityEngine;
using static PBF.PlayerBuffBase;

public class PlayerBuff : MonoBehaviour
{
    public PlayerController player;

    //-------------------------------------------
    //�e�o�t�Ɋւ���ϐ�
    //-------------------------------------------
    //�K�E�Z�Q�[�W�����o�t�p�����[�^
    [SerializeField, Header("�K�E�Z�Q�[�W�o�t�Ɋւ���l")]
    private exAttackBuff exGage = new() { getBuffCount = 0 };

    //�ړ����x�����o�t�p�����[�^
    [SerializeField, Header("�X�s�[�h�A�b�v�o�t�Ɋւ���l")]
    private speedBuff speed = new() { getBuffCount = 0 };

    //�a���ǉ��o�t�p�����[�^
    [SerializeField, Header("�a���ǉ��o�t�Ɋւ���l")]
    private slashingBuff slashing = new() { getBuffCount = 0 };

    //���G���o�t�p�����[�^
    [SerializeField, Header("���G���o�t�Ɋւ���l")]
    private invincibleBuff invincible = new() { getBuffCount = 0 };
    //----------------------------------------------

    //�v���C���[�X�e�[�^�X�����l�i�[�ϐ�
    float moveFirstSpeed;
    float moveDashSpeed;
    float moveMaxSpeed;
    float jumpSpeed;

    //�����o�t���ʗʊi�[�ϐ�
    exAttackBuff firstExAtBuff;
    speedBuff firstSpeedBuuf;
    slashingBuff firstSlashingBuff;
    invincibleBuff firstInvincibleBuff;

    public static PlayerBuff Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        FirstBuffKeep();
    }

    /// <summary>
    /// �K�E�Z�Q�[�W�����o�t
    /// </summary>
    public void ExAttackGageUp()
    {
        //�Q�[�W�ǉ��i�l���� - (�l�������� �~ �l����)
        for(int i = 0; i < exGage.setBuffNum - (exGage.setBuffDown * exGage.getBuffCount); i++) {
            ExAttackParam.Instance.AddGauge();
        }
        exGage.getBuffCount++;
    }

    /// <summary>
    /// ���x�㏸�o�t
    /// </summary>
    public void SpeedUp()
    {
        if (player.gameObject.GetComponent<SpeedUp>())
        {
            player.gameObject.GetComponent<SpeedUp>().AddBuff();
        }
        else
        {
            //�v���C���[�igameObject)�Ɏa���ǉ��X�N���v�g��ǉ�
            player.gameObject.AddComponent<SpeedUp>();
        }
    }
    public speedBuff GetSpeed()
    {
        return speed;
    }

    /// <summary>
    /// �a���ǉ��o�t
    /// </summary>
    public void SlashingBuff()
    {
        if (player.gameObject.GetComponent<SlashingBuff>())
        {
            player.gameObject.GetComponent<SlashingBuff>().AddBuff(slashing.getBuffCount);
        }
        else 
        {
            //�v���C���[�igameObject)�Ɏa���ǉ��X�N���v�g��ǉ�
            player.gameObject.AddComponent<SlashingBuff>();
        } 
        slashing.getBuffCount++;
    }
    public slashingBuff GetSlashing()
    {
        return slashing;
    }

    /// <summary>
    /// ���G���o�t
    /// </summary>
    public void InvincibleBuff()
    {
        if (player.gameObject.GetComponent<SlashingBuff>())
        {
            //�v���C���[�igameObject)�ɖ��G���X�N���v�g��ǉ�
            player.gameObject.AddComponent<InvinciblBuff>().AddBuff(invincible.getBuffCount);
        }
        else
        {
            //�v���C���[�igameObject)�ɖ��G���X�N���v�g��ǉ�
            player.gameObject.AddComponent<InvinciblBuff>();
        }
        invincible.getBuffCount++;
    }
    public invincibleBuff GetInvincible()
    {
        return invincible;
    }

    /// <summary>
    /// �o�t�t�^�O�̊e���l�ۑ�
    /// </summary>
    protected void FirstBuffKeep()
    {
        firstExAtBuff = exGage;
        firstSpeedBuuf = speed;
        firstSlashingBuff = slashing;
        firstInvincibleBuff = invincible;

        moveFirstSpeed = player.moveData.firstSpeed;
        moveDashSpeed = player.moveData.dashSpeed;
        moveMaxSpeed = player.moveData.maxSpeed;
        jumpSpeed = player.jumpData.speed;
    }

    /// <summary>
    /// �o�t���Z�b�g
    /// </summary>
    public void BuffRest()
    {
        exGage = firstExAtBuff;
        speed = firstSpeedBuuf;
        slashing = firstSlashingBuff;
        invincible = firstInvincibleBuff;

        player.moveData.firstSpeed = moveFirstSpeed;
        player.moveData.dashSpeed = moveDashSpeed;
        player.moveData.maxSpeed = moveMaxSpeed;
        player.jumpData.speed = jumpSpeed;
    }

    public float GetPlayerMoveData()
    {
        return moveFirstSpeed;
    }
}
