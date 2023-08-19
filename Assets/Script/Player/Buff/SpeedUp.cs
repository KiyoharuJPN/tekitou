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
        player = gameObject.GetComponent<PlayerController>();
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
        if (speed.getBuffCount % 2 == 0 && speed.setBuffNum > 1)
        {
            speed.setBuffNum -= speed.setBuffDown;
        }

        player.moveData.firstSpeed += speed.setBuffNum;
        player.moveData.dashSpeed += speed.setBuffNum;
        player.moveData.maxSpeed += speed.setBuffNum;
        player.moveData.jumpFirstSpeed += speed.setBuffNum;
    }

}
