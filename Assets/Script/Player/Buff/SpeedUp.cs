using UnityEngine;
using static PBF.PlayerBuffBase;
using Unity.VisualScripting;
using AIE2D;

public class SpeedUp : MonoBehaviour
{
    SpeedBuff speed;
    SpriteGlow.SpriteGlowEffect spriteGlow;
    PlayerController player;

    //バフ中に光る色
    Color32 color = Color.cyan;//水色

    void Start()
    {
        speed = PlayerBuff.Instance.GetSpeed();
        player = this.gameObject.GetComponent<PlayerController>();
        gameObject.GetComponent<StaticAfterImageEffect2DPlayer>().enabled = true;
        spriteGlow = gameObject.GetComponent<SpriteGlow.SpriteGlowEffect>();

        AddBuff();
        
        if (!gameObject.GetComponent<InvinciblBuff>() || !gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.GlowColor = color;
        }
    }

    public void AddBuff()
    {
        if (PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) > 10) return;

        if (PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 3 ||
            PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 5 ||
            PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 7 ||
            PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 9)
        {
            speed.setBuffNum -= speed.setBuffDown;
        }

        //移動増加
        player.moveData.firstSpeed += speed.setBuffNum;
        player.moveData.dashSpeed += speed.setBuffNum;
        player.moveData.maxSpeed += speed.setBuffNum;
        player.moveData.jumpFirstSpeed += speed.setBuffNum;

        //攻撃速度増加
        player.animSpeed += speed.attackSpeedNum;
        player.animator.SetFloat("Speed", player.animSpeed);
    }
}
