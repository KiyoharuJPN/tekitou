using Unity.VisualScripting;
using UnityEngine;
using static PBF.PlayerBuffBase;

public class PlayerBuff : MonoBehaviour
{
    public PlayerController player;

    //-------------------------------------------
    //各バフに関する変数
    //-------------------------------------------
    //必殺技ゲージ増加バフパラメータ
    [SerializeField, Header("必殺技ゲージバフに関する値")]
    private exAttackBuff exGage = new() { getBuffCount = 0 };

    //移動速度増加バフパラメータ
    [SerializeField, Header("スピードアップバフに関する値")]
    private speedBuff speed = new() { getBuffCount = 0 };

    //斬撃追加バフパラメータ
    [SerializeField, Header("斬撃追加バフに関する値")]
    private slashingBuff slashing = new() { getBuffCount = 0 };

    //無敵化バフパラメータ
    [SerializeField, Header("無敵化バフに関する値")]
    private invincibleBuff invincible = new() { getBuffCount = 0 };
    //----------------------------------------------

    //プレイヤーステータス初期値格納変数
    float moveFirstSpeed;
    float moveDashSpeed;
    float moveMaxSpeed;
    float jumpSpeed;

    //初期バフ効果量格納変数
    exAttackBuff firstExAtBuff;
    speedBuff firstSpeedBuuf;
    slashingBuff firstSlashingBuff;
    invincibleBuff firstInvincibleBuff;

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
    /// 必殺技ゲージ増加バフ
    /// </summary>
    public void ExAttackGageUp()
    {
        //ゲージ追加（獲得量 - (獲得毎減少 × 獲得回数)
        for(int i = 0; i < exGage.setBuffNum - (exGage.setBuffDown * exGage.getBuffCount); i++) {
            ExAttackParam.Instance.AddGauge();
        }
        exGage.getBuffCount++;
    }

    /// <summary>
    /// 速度上昇バフ
    /// </summary>
    public void SpeedUp()
    {
        if (player.gameObject.GetComponent<SpeedUp>())
        {
            player.gameObject.GetComponent<SpeedUp>().AddBuff();
        }
        else
        {
            //プレイヤー（gameObject)に斬撃追加スクリプトを追加
            player.gameObject.AddComponent<SpeedUp>();
        }
    }
    public speedBuff GetSpeed()
    {
        return speed;
    }

    /// <summary>
    /// 斬撃追加バフ
    /// </summary>
    public void SlashingBuff()
    {
        if (player.gameObject.GetComponent<SlashingBuff>())
        {
            player.gameObject.GetComponent<SlashingBuff>().AddBuff(slashing.getBuffCount);
        }
        else 
        {
            //プレイヤー（gameObject)に斬撃追加スクリプトを追加
            player.gameObject.AddComponent<SlashingBuff>();
        } 
        slashing.getBuffCount++;
    }
    public slashingBuff GetSlashing()
    {
        return slashing;
    }

    /// <summary>
    /// 無敵化バフ
    /// </summary>
    public void InvincibleBuff()
    {
        if (player.gameObject.GetComponent<SlashingBuff>())
        {
            //プレイヤー（gameObject)に無敵化スクリプトを追加
            player.gameObject.AddComponent<InvinciblBuff>().AddBuff(invincible.getBuffCount);
        }
        else
        {
            //プレイヤー（gameObject)に無敵化スクリプトを追加
            player.gameObject.AddComponent<InvinciblBuff>();
        }
        invincible.getBuffCount++;
    }
    public invincibleBuff GetInvincible()
    {
        return invincible;
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

        moveFirstSpeed = player.moveData.firstSpeed;
        moveDashSpeed = player.moveData.dashSpeed;
        moveMaxSpeed = player.moveData.maxSpeed;
        jumpSpeed = player.jumpData.speed;
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

    public float GetPlayerMoveData()
    {
        return moveFirstSpeed;
    }
}
