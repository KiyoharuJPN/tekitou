using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//BGM・SE管理
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

            //勝手に修正が入らないためリリース版でコメントアウト或いは削除してください。
            masterVolume = 0.4f;
            seMasterVolume = 0.4f;
            bgmMasterVolume = 0.4f;
            //勝手に修正が入らないためリリース版でコメントアウト或いは削除してください。
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

    //BGM終了確認
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
        //Stage2,//削除
        Result,
        GameOver,
        KingSlimeBoss_intro,
        KingSlimeBoss_roop,
        //GoalBGM,//削除
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
        //ShootMagicBall,       //削除
        //ForefootHeavyAttack,  //削除
/*        DragonRoar,             //ドラゴンの咆哮（実質削除）
        DragonBlaze,            //炎を吐く（実質削除）
        RockDropOff,            //岩の落下（実質削除）
        RockBreak,              //岩を砕ける音（実質削除）
        WaggingTailPowerful,    //勢いよくしっぽを振る（実質削除）*/
        BirdChirping,           //鳥の鳴き声
        Door,                   //扉


        //BGMイントロ


    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}