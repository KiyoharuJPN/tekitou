using UnityEngine;
using System.Collections;
using static PBF.PlayerBuffBase;

public class InvinciblBuff : MonoBehaviour
{
    InvincibleBuff invincible;
    float buffTime;

    [SerializeField]
    GameObject invincibleObj;

    SpriteGlow.SpriteGlowEffect spriteGlow;
    //バフ中に光る色
    Color32 color = Color.yellow;//黄色

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("InvinciblBuffStart");
        gameObject.tag = "InvinciblePlayer";
        invincible = PlayerBuff.Instance.GetInvincible();
        buffTime = invincible.firstSetTime;
        invincibleObj = this.transform.Find("Invincibility").gameObject;
        invincibleObj.SetActive(true);
        
        spriteGlow = gameObject.GetComponent<SpriteGlow.SpriteGlowEffect>();

        spriteGlow.GlowColor = color;

        StartCoroutine(InvincibleMode());
    }

    internal void AddBuff(int count)
    {
        buffTime += invincible.buffSetTime - invincible.buffTimeDown * count;
    }

    IEnumerator InvincibleMode()
    {
        while (buffTime > 0)
        {
            if (this.gameObject.GetComponent<PlayerController>().canMove)
            {
                buffTime -= Time.deltaTime;
            }
            yield return null;
        }
        invincibleObj.SetActive(false);
        

        if (gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.GlowColor = Color.green;
        }
        else if (gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.GlowColor = Color.cyan;
        }
        else if(!gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.EnableInstancing = true;
        }

        PlayerBuff.Instance.CountReset_Invincible();
        gameObject.tag = "Player";
        GameManager.Instance.BGMBack();
        Destroy(this.GetComponent<InvinciblBuff>());
    }
}
