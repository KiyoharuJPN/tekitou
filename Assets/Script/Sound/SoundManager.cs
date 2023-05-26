using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BGM�ESE�Ǘ�
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;

    [SerializeField] List<BGMSoundData> bgmSoundDatas;
    [SerializeField] List<SESoundData> seSoundDatas;

    public float masterVolume { get; set; }
    public float bgmMasterVolume { get; set; }
    public float seMasterVolume { get; set; }

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //����ɏC��������Ȃ����߃����[�X�łŃR�����g�A�E�g�����͍폜���Ă��������B
            masterVolume = 0.4f;
            seMasterVolume = 0.4f;
            bgmMasterVolume = 0.4f;
            //����ɏC��������Ȃ����߃����[�X�łŃR�����g�A�E�g�����͍폜���Ă��������B
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void PlayBGM(BGMSoundData.BGM bgm)
    {
        BGMSoundData data = bgmSoundDatas.Find(data => data.bgm == bgm);
        bgmAudioSource.clip = data.audioClip;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();
    }

    public void BGMLoopSwich()
    {
        if (bgmAudioSource.loop)
        {
            bgmAudioSource.loop = false;
        }
        else if(!bgmAudioSource.loop)
        {
            bgmAudioSource.loop = true;
        }
    }

    //BGM�I���m�F
    public bool BGMEnd()
    {
        if (bgmAudioSource.isPlaying)
        {
            return true; 
        }
        else
        {
            return false;
        }
    }


    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(data => data.se == se);
        /*if(data != null)
        {
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.PlayOneShot(data.audioClip);
        }*/
        seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
        seAudioSource.PlayOneShot(data.audioClip);
    }

}

//BGM���X�g
[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        Title,
        Tutorial_intro,
        Tutorial_roop,
        Stage1_intro,
        Stage1_roop,
        //Stage2,//�폜
        Result,
        GameOver,
        KingSlimeBoss_intro,
        KingSlimeBoss_roop,
        //GoalBGM,//�폜
    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}



//SE���X�g
[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        //�v���C���[�T�E���h
        Dash,                   //�_�b�V���I��
        Jump,                   //���
        AirJump,                //��Jump
        AutoAttack,�@           //�����U��
        UpAttack,               //�㏸�U��
        DropAttackStart,        //���~�U���J�n
        //DropAttack,�@�@�@       //���~�U��
        DropAttackFall,         //���~�U��������
        DropAttackLand,         //���~�U�����n��
        SideAttack,             //���U��
        PlayerGetHit,           //��e
        PlayerDead,             //���S��
        ExAttack_CutIn,         //�K�E�Z�g�p��
        ExAttack_Wind,          //�匕���؂艹
        ExAttack_Hit,           //�匕�U���q�b�g��
        ExAttack_PowerCharge,   //�͂��`���[�W
        //LastAttack,             //�Ō�̈ꌂ�i�����폜�j
        //����
        GoalSE,                 //�S�[����
        GetHeart,               //�n�[�g�擾
        GetCoin,                //�R�C���擾
        //�����X�^�[�T�E���h
        MonsterGetHit,          //�G��e
        MonsterKnock,           //�������
        MonsterDead,            //�G����
        SwingDownClub,          //����_��U�艺�낷
        //CutWithNails,           //�܂ł���i�����폜�j
        //HeavyLand,              //�n�ʂɋ������n
        KingSlimeLanding,       //�L���O�X���C�����n
        KingSlimeSummon,        //�X���C������
        BossDown,               //�{�X���j
        //ShootMagicBall,       //�폜
        //ForefootHeavyAttack,  //�폜
/*        DragonRoar,             //�h���S���̙��K�i�����폜�j
        DragonBlaze,            //����f���i�����폜�j
        RockDropOff,            //��̗����i�����폜�j
        RockBreak,              //����ӂ��鉹�i�����폜�j
        WaggingTailPowerful,    //�����悭�����ۂ�U��i�����폜�j*/
        BirdChirping,           //���̖���
        Door,                   //��


        //BGM�C���g��


    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}