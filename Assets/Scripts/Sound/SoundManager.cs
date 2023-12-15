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

    public float masterVolume;
    public float bgmMasterVolume;
    public float seMasterVolume;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            introAudioSource = bgmAudioSource;
            loopAudioSource = bgmAudioSource;

            introAudioSource.loop = false;
            introAudioSource.playOnAwake = false;

            loopAudioSource.loop = true;
            loopAudioSource.playOnAwake = false;
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
            if(bgm_intro == BGMSoundData.BGM.Result)
            {
                bgmAudioSource.loop = false;
            }
            bgmAudioSource.volume = bgmMasterVolume * masterVolume;
            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.Play();
        }
        else
        {
            BGMSoundData data_intro = bgmSoundDatas.Find(data => data.bgm == bgm_intro);
            BGMSoundData data_loop = bgmSoundDatas.Find(data => data.bgm == bgm_loop);

            introAudioSource.clip = data_intro.audioClip;
            loopAudioSource.clip = data_loop.audioClip;

            bgmAudioSource.volume = bgmMasterVolume * masterVolume;
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
        //if (data != null)
        //{
        //    seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
        //    seAudioSource.PlayOneShot(data.audioClip);
        //}
        seAudioSource.volume = seMasterVolume * masterVolume;
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
        none, //Result�̗l�ȃC���g���E���[�v���Ȃ�BGM���Ăяo���ۂ̈����Q
        Stage2_intro,
        Stage2_roop,
        //���G��BGM
        Invincibility,
        Stage3,
        Stage3_Boss_intro,
        Stage3_Boss_roop
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
        //
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
        ShootMagicBall,         //���@�̒e���΂�
        ForefootHeavyAttack,    //�O���𐨂��悭�U�艺�낷�i�S�u�����U�����g�p�j
        DragonRoar,             //�h���S���̙��K
        DragonBlaze,            //����f��
        RockDropOff,            //��̗���
        RockBreak,              //����ӂ��鉹
        WaggingTailPowerful,    //�����悭�����ۂ�U��
        BirdChirping,           //���̖���
        Door,                   //��
        HalfPoint,              //���Ԓn�_�N��

        //�`���[�g���A��������
        tutorialCorrect,

        //������
        BuffGet,
        SlashingWave,

        //�X�e�[�W�R
        DemonkingShout,
        //�����ǋN����
        moveWall,
        //ExGageMax
        exGageMax,

        //TODO �Z�C�J�t�F�X����R�[�h
        Seika_Jump,
        Seika_AirJump,

        TimeResult
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}