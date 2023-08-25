using System.Collections;
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
    private ExAttackBuff exGage = new() { getBuffCount = 0 };

    //�ړ����x�����o�t�p�����[�^
    [SerializeField, Header("�X�s�[�h�A�b�v�o�t�Ɋւ���l")]
    private SpeedBuff speed = new() { getBuffCount = 0 };

    //�a���ǉ��o�t�p�����[�^
    [SerializeField, Header("�a���ǉ��o�t�Ɋւ���l")]
    private PBF.PlayerBuffBase.SlashingBuff slashing = new() { getBuffCount = 0 };

    //���G���o�t�p�����[�^
    [SerializeField, Header("���G���o�t�Ɋւ���l")]
    private InvincibleBuff invincible = new() { getBuffCount = 0 };
    //----------------------------------------------

    //�v���C���[�X�e�[�^�X�����l�i�[�ϐ�
    float moveFirstSpeed;
    float moveDashSpeed;
    float moveMaxSpeed;
    float jumpSpeed;

    //�����o�t���ʗʊi�[�ϐ�
    ExAttackBuff firstExAtBuff;
    SpeedBuff firstSpeedBuuf;
    PBF.PlayerBuffBase.SlashingBuff firstSlashingBuff;
    InvincibleBuff firstInvincibleBuff;

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
        player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
        //�Q�[�W�ǉ��i�l���� - (�l�������� �~ �l����)
        for (int i = 0; i < exGage.setBuffNum - (exGage.setBuffDown * exGage.getBuffCount); i++) {
            ExAttackParam.Instance.AddGauge();
        }
        exGage.getBuffCount++;

        if(!player.gameObject.GetComponent<SpeedUp>() && 
            !player.gameObject.GetComponent<SlashingBuff>() && 
            !player.gameObject.GetComponent<InvinciblBuff>())
        {
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().GlowColor = new Color(1f, 0.05f, 0, 1);
            StartCoroutine(ColorChenge());
        }
    }
    IEnumerator ColorChenge()
    {
        float time = 0.2f;
        while (time > 0) { 
            time -= Time.deltaTime; 
            yield return null;
        }

        if (!player.gameObject.GetComponent<SpeedUp>() ||
            !player.gameObject.GetComponent<SlashingBuff>() ||
            !player.gameObject.GetComponent<InvinciblBuff>())
        {
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = true;
        }
    }

    /// <summary>
    /// ���x�㏸�o�t
    /// </summary>
    public void SpeedUp()
    {
        speed.getBuffCount++;
        if (player.gameObject.GetComponent<SpeedUp>())
        {
            player.gameObject.GetComponent<SpeedUp>().AddBuff();
        }
        else
        {
            //�v���C���[�igameObject)�Ɏa���ǉ��X�N���v�g��ǉ�
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
            player.gameObject.AddComponent<SpeedUp>();
        }
    }
    public SpeedBuff GetSpeed()
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
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
            player.gameObject.AddComponent<SlashingBuff>();
        } 
        slashing.getBuffCount++;
    }
    public PBF.PlayerBuffBase.SlashingBuff GetSlashing()
    {
        return slashing;
    }
    public void CountReset_Slashing()
    {
        slashing.getBuffCount = 0;
    }

    /// <summary>
    /// ���G���o�t
    /// </summary>
    public void InvincibleBuff()
    {
        if (player.gameObject.GetComponent<InvinciblBuff>())
        {
            //�v���C���[�igameObject)�ɖ��G���X�N���v�g��ǉ�
            player.gameObject.GetComponent<InvinciblBuff>().AddBuff(invincible.getBuffCount);
        }
        else
        {
            //�v���C���[�igameObject)�ɖ��G���X�N���v�g��ǉ�
            SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Invincibility, BGMSoundData.BGM.none);
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
            player.gameObject.AddComponent<InvinciblBuff>();
        }
        invincible.getBuffCount++;
    }
    public InvincibleBuff GetInvincible()
    {
        return invincible;
    }
    public void CountReset_Invincible()
    {
        invincible.getBuffCount = 0;
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

    public int GetBuffCount(BuffType buffType)
    {
        switch(buffType)
        {
            case BuffType.ExGage:
                return exGage.getBuffCount;
            case BuffType.SpeedUp:
                return speed.getBuffCount;
            case BuffType.Slashing:
                return slashing.getBuffCount;
            case BuffType.Invincible:
                return invincible.getBuffCount;
        }
        return 0;
    }

    public float GetPlayerMoveData()
    {
        return moveFirstSpeed;
    }
}
