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
        if(data != null)
        {
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.PlayOneShot(data.audioClip);
        }
    }

}

//BGM���X�g
[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        Title,
        Tutorial,
        Stage1_intro,
        Stage1_roop,
        Stage2,
        GoalBGM,
        GameOver,
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
        DropAttack,�@�@�@       //���~�U��
        DropAttackLand,         //���~�U�����n��
        SideAttack,
        PlayerGetHit,
        PlayerDead,
        ExAttack_CutIn,
        ExAttack_Wind,
        ExAttack_Hit,
        ExAttack_PowerCharge,
        PowerCharge,
        LastAttack,
        //����
        GoalSE,
        GetHeart,
        GetCoin,
        //�����X�^�[�T�E���h
        MonsterGetHit,
        MonsterKnock,
        MonsterDead,
        ClawToKill,
        HeavyLand,
        SummonSlime,
        BossDown,
        ShootMagicBall,
        ForefootHeavyAttack,
        DragonRoar,
        DragonBlaze,
        RockDropOff,
        RockBreak,
        KingSlimeLanding,
        //BGM�C���g��
        
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}