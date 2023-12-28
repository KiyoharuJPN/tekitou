using UnityEngine;
using System.Collections;
using static PBF.PlayerBuffBase;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SlashingBuff : MonoBehaviour
{
    PBF.PlayerBuffBase.SlashingBuff slashing;
    float buffTime;

    SpriteGlow.SpriteGlowEffect spriteGlow;
    //バフ中に光る色
    Color32 color = Color.green;//緑

    //斬撃のスピード
    float waveSpeed;

    //残り時間のバー
    BuffTimer timeBar;
    float time;

    //最低時間
    const float miniTime = 10f;

    public enum SlashingType
    {
        sideAttack_Right,
        sideAttack_Left,
        UpAttack,
        DropAttack
    }

    void Start()
    {
        slashing = PlayerBuff.Instance.GetSlashing();
        buffTime = slashing.firstSetTime;
        spriteGlow = gameObject.GetComponent<SpriteGlow.SpriteGlowEffect>();

        //残り時間のバー表示・設定
        timeBar = GameObject.Find("PlayerBuffTime").GetComponent<BuffTimer>();
        timeBar.GetComponent<UIPosController>().enabled = true;
        timeBar.GetComponent<Canvas>().enabled = true;

        waveSpeed = slashing.slashingSpeed;

        if (!gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.GlowColor = color;
        }

        StartCoroutine(SlashingMode());
    }

    //斬撃生成メソッド
    public void Slashing(SlashingType type, GameObject player)
    {
        switch (type)
        {
            case SlashingType.sideAttack_Right:
                SlashingWaveGenerate(player.transform.position + new Vector3(1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), false, new Vector2(waveSpeed, 0));
                break;
            case SlashingType.sideAttack_Left:
                SlashingWaveGenerate(player.transform.position + new Vector3(-1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), true, new Vector2(-waveSpeed, 0));
                break;
            case SlashingType.UpAttack:
                SlashingWaveGenerate(player.transform.position + new Vector3(0, 1f, 0),
                    Quaternion.Euler(0f, 0f, 90f), false, new Vector2(0, waveSpeed));
                break;
            case SlashingType.DropAttack:
                SlashingWaveGenerate(player.transform.position + new Vector3(1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), false, new Vector2(waveSpeed, 0));
                SlashingWaveGenerate(player.transform.position + new Vector3(-1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), true, new Vector2(-waveSpeed, 0));
                break;
        }

    }

    //斬撃波生成(第一引数:生成場所、第二引数:角度、第三引数:X反転の有無、第四引数:力)
    private void SlashingWaveGenerate(Vector3 position, Quaternion rotation, bool flipX, Vector2 velocity)
    {
        slashing = PlayerBuff.Instance.GetSlashing();
        SoundManager.Instance.PlaySE(SESoundData.SE.SlashingWave);
        GameObject obj = Instantiate(slashing.slashingObj, position, Quaternion.identity);
        obj.transform.rotation = rotation;
        obj.GetComponent<SlashingWave>().player = this.gameObject.GetComponent<PlayerController>();

        obj.GetComponent<SpriteRenderer>().flipX = flipX;

        obj.GetComponent<Rigidbody2D>().velocity = velocity;
    }

    public void SlashingWaveRemove()
    {
        if (gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.GlowColor = Color.cyan;
        }
        if (!gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.EnableInstancing = true;
        }

        timeBar.GetComponent<UIPosController>().enabled = false;
        timeBar.GetComponent<Canvas>().enabled = false;

        PlayerBuff.Instance.CountReset_Slashing();
        Destroy(this.GetComponent<SlashingBuff>());
    }

    internal void AddBuff(int count)
    {
        timeBar = GameObject.Find("PlayerBuffTime").GetComponent<BuffTimer>();
        float addTime;
        if (miniTime > slashing.buffSetTime - slashing.buffTimeDown * count)
        {
            addTime = miniTime;
        }
        else
        {
            addTime = slashing.buffSetTime - slashing.buffTimeDown * count;
        }
        buffTime += addTime;
        time += addTime;

        //バー再セット
        timeBar.BarSet(1);
    }

    IEnumerator SlashingMode()
    {
        time = buffTime;

        while (time > 0)
        {
            if (this.gameObject.GetComponent<PlayerController>().canMove)
            {
                time -= Time.deltaTime;
                timeBar.BarSet(time / buffTime);
            }
            yield return null;
        };

        if (gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.GlowColor = Color.cyan;
        }
        if (!gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.EnableInstancing = true;
        }

        timeBar.GetComponent<UIPosController>().enabled = false;
        timeBar.GetComponent<Canvas>().enabled = false;

        PlayerBuff.Instance.CountReset_Slashing();
        Destroy(this.GetComponent<SlashingBuff>());
    }
}
