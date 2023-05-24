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
        if(data != null)
        {
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.PlayOneShot(data.audioClip);
        }
    }

}

//BGMリスト
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
        DropAttack,　　　       //下降攻撃
        DropAttackLand,         //下降攻撃着地時
        SideAttack,
        PlayerGetHit,
        PlayerDead,
        ExAttack_CutIn,
        ExAttack_Wind,
        ExAttack_Hit,
        ExAttack_PowerCharge,
        PowerCharge,
        LastAttack,
        //環境音
        GoalSE,
        GetHeart,
        GetCoin,
        //モンスターサウンド
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
        //BGMイントロ
        
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}