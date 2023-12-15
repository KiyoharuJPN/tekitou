using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BGM・SE管理
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
    //注意：Resetの様にイントロ・ループが存在しない場合は、bgm_loopをnoneで呼び出す
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

//BGMリスト
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
        none, //Resultの様なイントロ・ループがないBGMを呼び出す際の引数２
        Stage2_intro,
        Stage2_roop,
        //無敵中BGM
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



//SEリスト
[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        //プレイヤーサウンド
        Dash,                   //ダッシュオン
        Jump,                   //飛ぶ
        AirJump,                //空中Jump
        AutoAttack,　           //自動攻撃
        UpAttack,               //上昇攻撃
        DropAttackStart,        //下降攻撃開始
        //DropAttack,　　　       //下降攻撃
        DropAttackFall,         //下降攻撃落下中
        DropAttackLand,         //下降攻撃着地時
        SideAttack,             //横攻撃
        PlayerGetHit,           //被弾
        PlayerDead,             //死亡時
        ExAttack_CutIn,         //必殺技使用時
        ExAttack_Wind,          //大剣風切り音
        ExAttack_Hit,           //大剣攻撃ヒット音
        ExAttack_PowerCharge,   //力をチャージ
        //LastAttack,             //最後の一撃（実質削除）
        //環境音
        GoalSE,                 //ゴール時
        GetHeart,               //ハート取得
        GetCoin,                //コイン取得
        //
        //モンスターサウンド
        MonsterGetHit,          //敵被弾
        MonsterKnock,           //吹き飛び
        MonsterDead,            //敵消滅
        SwingDownClub,          //こん棒を振り下ろす
        //CutWithNails,           //爪できる（実質削除）
        //HeavyLand,              //地面に強く着地
        KingSlimeLanding,       //キングスライム着地
        KingSlimeSummon,        //スライム召喚
        BossDown,               //ボス撃破
        ShootMagicBall,         //魔法の弾を飛ばす
        ForefootHeavyAttack,    //前足を勢いよく振り下ろす（ゴブリン攻撃音使用）
        DragonRoar,             //ドラゴンの咆哮
        DragonBlaze,            //炎を吐く
        RockDropOff,            //岩の落下
        RockBreak,              //岩を砕ける音
        WaggingTailPowerful,    //勢いよくしっぽを振る
        BirdChirping,           //鳥の鳴き声
        Door,                   //扉
        HalfPoint,              //中間地点起動

        //チュートリアル正解音
        tutorialCorrect,

        //強化時
        BuffGet,
        SlashingWave,

        //ステージ３
        DemonkingShout,
        //動く壁起動音
        moveWall,
        //ExGageMax
        exGageMax,

        //TODO セイカフェス限定コード
        Seika_Jump,
        Seika_AirJump,

        TimeResult
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}