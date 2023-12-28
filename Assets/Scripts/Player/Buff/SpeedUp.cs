using UnityEngine;
using static PBF.PlayerBuffBase;
using Unity.VisualScripting;
using AIE2D;

public class SpeedUp : MonoBehaviour
{
    SpeedBuff speed;
    SpriteGlow.SpriteGlowEffect spriteGlow;
    PlayerController player;

    //�o�t���Ɍ���F
    Color32 color = Color.cyan;//���F

    private void Awake()
    {
        speed = PlayerBuff.Instance.GetSpeed();
    }

    void Start()
    {
        gameObject.GetComponent<StaticAfterImageEffect2DPlayer>().enabled = true;
        spriteGlow = gameObject.GetComponent<SpriteGlow.SpriteGlowEffect>();

        AddBuff();

        if (!gameObject.GetComponent<InvinciblBuff>() && !gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.GlowColor = color;
        }
    }

    public void AddBuff()
    {
        player = this.gameObject.GetComponent<PlayerController>();
        if (PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) > 10) return;

        if (PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 3 ||
            PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 5 ||
            PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 7 ||
            PlayerBuff.Instance.GetBuffCount(BuffType.SpeedUp) == 9)
        {
            speed.setBuffNum -= speed.setBuffDown;
        }

        //�ړ�����
        player.moveData.firstSpeed += speed.setBuffNum;
        player.moveData.dashSpeed += speed.setBuffNum;
        player.moveData.maxSpeed += speed.setBuffNum;
        player.moveData.jumpFirstSpeed += speed.setBuffNum;

        //�U�����x����
        player.animSpeed += speed.attackSpeedNum;
        if(player.animSpeed > speed.maxAttackSpeed) 
        { 
            player.animSpeed = speed.maxAttackSpeed;
        }
        player.animator.SetFloat("Speed", player.animSpeed);
    }
}
