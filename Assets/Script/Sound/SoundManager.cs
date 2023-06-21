using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BGM�ESE�Ǘ�
public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;

    AudioSource introAudioSource;
    AudioSource loopAudioSource;

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

            introAudioSource = bgmAudioSource;
            loopAudioSource = bgmAudioSource;

            introAudioSource.loop = false;
            introAudioSource.playOnAwake = false;

            loopAudioSource.loop = true;
            loopAudioSource.playOnAwake = false;

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

    //playBGM
    //���ӁFReset�̗l�ɃC���g���E���[�v�����݂��Ȃ��ꍇ�́Abgm_loop��none�ŌĂяo��
    public void PlayBGM(BGMSoundData.BGM bgm_intro, BGMSoundData.BGM bgm_loop)
    {
        if(bgm_loop == BGMSoundData.BGM.none)
        {
            BGMSoundData data = bgmSoundDatas.Find(data => data.bgm == bgm_intro);
            bgmAudioSource.loop = false;
            bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.Play();
        }
        else
        {
            BGMSoundData data_intro = bgmSoundDatas.Find(data => data.bgm == bgm_intro);
            BGMSoundData data_loop = bgmSoundDatas.Find(data => data.bgm == bgm_loop);

            introAudioSource.clip = data_intro.audioClip;
            loopAudioSource.clip = data_loop.audioClip;

            bgmAudioSource.volume = data_intro.volume * bgmMasterVolume * masterVolume;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            bgmAudioSource.PlayScheduled(AudioSettings.dspTime + loopAudioSource.clip.length);
        }
    }

    

    public void StopBGM()
    {
        if (introAudioSource == null || loopAudioSource == null)
        {
            return;
        }

        if (introAudioSource.isPlaying)
        {
            introAudioSource.Stop();
        }
        else if (loopAudioSource.isPlaying)
        {
            loopAudioSource.Stop();
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

    public bool isPlayBGM()
    {
        return bgmAudioSource.isPlaying;
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
        Result,
        GameOver_intro,
        GameOver_roop,
        KingSlimeBoss_intro,
        KingSlimeBoss_roop,
        none //Result�̗l�ȃC���g���E���[�v���Ȃ�BGM���Ăяo���ۂ̕Е�
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