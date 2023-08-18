using UnityEngine;
using System.Collections;
using static PBF.PlayerBuffBase;

public class InvinciblBuff : MonoBehaviour
{
    invincibleBuff invincible;
    float buffTime;

    GameObject invinvibleObj;

    // Start is called before the first frame update
    void Start()
    {
        invincible = PlayerBuff.Instance.GetInvincible();
        buffTime = invincible.firstSetTime;
        invinvibleObj = transform.Find("Invincibility").gameObject;
        invinvibleObj.SetActive(true);
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
        gameObject.layer = LayerMask.NameToLayer("PlayerAction");
        Destroy(this.GetComponent<InvinciblBuff>());
    }
}
