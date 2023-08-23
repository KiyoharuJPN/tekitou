using UnityEngine;
using System.Collections;
using static PBF.PlayerBuffBase;

public class InvinciblBuff : MonoBehaviour
{
    InvincibleBuff invincible;
    float buffTime;

    GameObject invinvibleObj;

    SpriteGlow.SpriteGlowEffect spriteGlow;
    //バフ中に光る色
    Color32 color = Color.yellow;//水色

    // Start is called before the first frame update
    void Start()
    {
        invincible = PlayerBuff.Instance.GetInvincible();
        buffTime = invincible.firstSetTime;
        invinvibleObj = transform.Find("Invincibility").gameObject;
        invinvibleObj.SetActive(true);
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
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
            buffTime -= Time.deltaTime;
            yield return null;
        }
        invinvibleObj.SetActive(false);
        

        if (gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.GlowColor = Color.green;
        }
        else if (gameObject.GetComponent<SpeedUp>())
        {
            spriteGlow.GlowColor = Color.cyan;
        }
        else if(!gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<SlashingBuff>())
        {
            spriteGlow.EnableInstancing = true;
        }

        gameObject.layer = LayerMask.NameToLayer("Player");
        PlayerBuff.Instance.CountReset_Invincible();
        GameManager.Instance.BGMBack();
        Destroy(this.GetComponent<InvinciblBuff>());
    }
}
