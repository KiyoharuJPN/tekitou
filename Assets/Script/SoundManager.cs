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


    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(data => data.se == se);
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
        Dungeon,
        Hoge,
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
        support_1, //誘導マーカー設置
        support_2, //射撃
        support_3, //バリア展開
        Fighter_Damage,　//戦闘機被弾
        Enemy_Bullet,    //敵の球発射
        Enemy_hit,　　　//敵撃破
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 0.4f;
}