using UnityEngine;
using static PBF.PlayerBuffBase;
using Unity.VisualScripting;
using AIE2D;

public class SpeedUp : MonoBehaviour
{
    speedBuff speed;
    PlayerController player;

    void Start()
    {
        speed = PlayerBuff.Instance.GetSpeed();
        player = gameObject.GetComponent<PlayerController>();
        gameObject.GetComponent<StaticAfterImageEffect2DPlayer>().enabled = true;

        AddBuff();
    }

    public void AddBuff()
    {
        speed.getBuffCount++;
        if (speed.getBuffCount % 2 == 0 && speed.setBuffNum > 1)
        {
            speed.setBuffNum -= speed.setBuffDown;
        }

        player.moveData.firstSpeed += speed.setBuffNum;
        player.moveData.dashSpeed += speed.setBuffNum;
        player.moveData.maxSpeed += speed.setBuffNum;
        player.moveData.jumpFirstSpeed += speed.setBuffNum;
        Debug.Log("スピードアップ!");
    }

}
