using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static PBF.PlayerBuffBase;

public class PlayerBuff : MonoBehaviour
{
    public PlayerController player;

    //-------------------------------------------
    //各バフに関する変数
    //-------------------------------------------
    //必殺技ゲージ増加バフパラメータ
    [SerializeField, Header("必殺技ゲージバフに関する値")]
    private ExAttackBuff exGage = new() { getBuffCount = 0 };

    //移動速度増加バフパラメータ
    [SerializeField, Header("スピードアップバフに関する値")]
    private SpeedBuff speed = new() { getBuffCount = 0 };

    //斬撃追加バフパラメータ
    [SerializeField, Header("斬撃追加バフに関する値")]
    private PBF.PlayerBuffBase.SlashingBuff slashing = new() { getBuffCount = 0 };

    //無敵化バフパラメータ
    [SerializeField, Header("無敵化バフに関する値")]
    private InvincibleBuff invincible = new() { getBuffCount = 0 };
    //----------------------------------------------

    //プレイヤーステータス初期値格納変数
    float moveFirstSpeed;
    float moveDashSpeed;
    float moveMaxSpeed;
    float jumpSpeed;

    //初期バフ効果量格納変数
    ExAttackBuff firstExAtBuff;
    SpeedBuff firstSpeedBuuf;
    PBF.PlayerBuffBase.SlashingBuff firstSlashingBuff;
    InvincibleBuff firstInvincibleBuff;

    [SerializeField, Header("バフ獲得時の画像")]
    Sprite[] buffImages;
    [SerializeField, Header("バフ獲得時UI")]
    GameObject getBuffUI;


    public static PlayerBuff Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        FirstBuffKeep();
    }

    /// <summary>
    /// 必殺技ゲージ増加バフ
    /// </summary>
    public void ExAttackGageUp()
    {
        GetBuffUIPop(0);
        player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
        //ゲージ追加（獲得量 - (獲得毎減少 × 獲得回数)
        int getExGage = exGage.setBuffNum - (exGage.setBuffDown * exGage.getBuffCount);
        if(getExGage < 5)
        {
            getExGage = 5;//最低値
        }
        for (int i = 0; i < getExGage; i++) {
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
    /// 速度上昇バフ
    /// </summary>
    public void SpeedUp()
    {
        GetBuffUIPop(1);
        speed.getBuffCount++;
        if (player.gameObject.GetComponent<SpeedUp>())
        {
            player.gameObject.GetComponent<SpeedUp>().AddBuff();
        }
        else
        {
            //プレイヤー（gameObject)にスピードアップスクリプトを追加
            player.GetComponent<SpriteGlow.SpriteGlowEffect>().EnableInstancing = false;
            player.gameObject.AddComponent<SpeedUp>();
        }
    }
    public SpeedBuff GetSpeed()
    {
        return speed;
    }

    /// <summary>
    /// 斬撃追加バフ
    /// </summary>
    public void SlashingBuff()
    {
        GetBuffUIPop(2);
        if (player.gameObject.GetComponent<SlashingBuff>())
        {
            player.gameObject.GetComponent<SlashingBuff>().AddBuff(slashing.getBuffCount);
        }
        else 
        {
            //プレイヤー（gameObject)に斬撃追加スクリプトを追加
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
    /// 無敵化バフ
    /// </summary>
    public void InvincibleBuff()
    {
        GetBuffUIPop(3);
        if (player.gameObject.GetComponent<InvinciblBuff>())
        {
            //プレイヤー（gameObject)に無敵化スクリプトを追加
            player.gameObject.GetComponent<InvinciblBuff>().AddBuff(invincible.getBuffCount);
        }
        else
        {
            //プレイヤー（gameObject)に無敵化スクリプトを追加
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
    /// バフ付与前の各数値保存
    /// </summary>
    protected void FirstBuffKeep()
    {
        firstExAtBuff = exGage;
        firstSpeedBuuf = speed;
        firstSlashingBuff = slashing;
        firstInvincibleBuff = invincible;
    }

    /// <summary>
    /// バフリセット
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

    void GetBuffUIPop(int getBuffNum)
    {
        var resultObj = Instantiate(getBuffUI, player.gameObject.transform.position + new Vector3(0f, 1.5f), Quaternion.identity);
        resultObj.GetComponent<GetBuffUI>().BuffImageSet(buffImages[getBuffNum]);
    }
}
