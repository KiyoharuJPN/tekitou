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
    //�o�t���Ɍ���F
    Color32 color = Color.yellow;//���F

    // Start is called before the first frame update
    void Start()
    {
        invincible = PlayerBuff.Instance.GetInvincible();
        buffTime = invincible.firstSetTime;
        invincibleObj = this.transform.Find("Invincibility").gameObject;
        invincibleObj.SetActive(true);
        gameObject.tag = "InvinciblePlayer";
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
        invincibleObj.SetActive(false);
        

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

        gameObject.tag = "Player";
        PlayerBuff.Instance.CountReset_Invincible();
        GameManager.Instance.BGMBack();
        Destroy(this.GetComponent<InvinciblBuff>());
    }
}
