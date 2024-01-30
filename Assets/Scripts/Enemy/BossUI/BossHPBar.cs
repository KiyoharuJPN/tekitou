using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class BossHPBar : MonoBehaviour
{
    public float FillSpeed = 1, DecreaseSpeed = 1;

    [Header("ゲージの画像")]
    [SerializeField]
    Image BossHPGauge;
    [SerializeField]
    Image BossHPBackGauge;

    Coroutine hpDecreasingCoro;

    float BossFullHP = 0, BossHP = 0;              //ボスのFullHP、ボスのHP仮記録用、HPの偏差値

    bool StartAnimation = false, HPdecreasing = false;      //開始アニメーションが流されているかどうかを確認、HPを変更するコルーチンが動いているかどうかの確認

    private void Start()
    {
        //初期化
        BossHPGauge.fillAmount = 0f;
        BossHPBackGauge.fillAmount = 0f;
        FillSpeed *= 0.001f;
        DecreaseSpeed *= 0.001f;
    }

    private void OnEnable()
    {
        //有効化される時に初期化をする
        StartAnimation = true;
        BossHPGauge.fillAmount = 0f;
        BossHPBackGauge.fillAmount = 0f;
        StartCoroutine(SetFullHP());
    }

    IEnumerator SetFullHP()
    {
        //初期化アニメーション
        while (BossHPGauge.fillAmount < 1)
        {
            BossHPGauge.fillAmount += FillSpeed;
            BossHPBackGauge.fillAmount += FillSpeed;
            yield return new WaitForEndOfFrame();
        }
        //初期化
        BossHP = BossFullHP = GetComponentInParent<Enemy>(true).GetEnemyFullHP();
        //BossHP = GetComponentInParent<Enemy>(true).GetEnemyHP();
        StartAnimation = false;
        ReductionHP();
    }

    //HP減少時に呼ぶコマンド
    public void ReductionHP()
    {
        if (!StartAnimation)
        {
            var currentHP = GetComponentInParent<Enemy>().GetEnemyHP();
            //殺される瞬間にHPゲージを消す
            if(currentHP <= 0)
            {
                gameObject.SetActive(false);
                return;
            }
            //HPが変わった場合は今のHPを表示して、減少アニメーションを流す。
            if (currentHP != BossHP)
            {
                //今のHPを更新する
                BossHP = currentHP;
                BossHPGauge.fillAmount = BossHP / BossFullHP;
                //HP減少アニメーションを流す
                if (!HPdecreasing)
                {
                    if(hpDecreasingCoro!=null)StopCoroutine(hpDecreasingCoro);
                    hpDecreasingCoro = StartCoroutine(DecreaseHP());
                }
            }
        }
    }

    //HPの背景部分のアニメーション
    IEnumerator DecreaseHP()
    {
        yield return new WaitForSeconds(0.7f);
        HPdecreasing = true;
        while(BossHPBackGauge.fillAmount > BossHPGauge.fillAmount)
        {
            if((BossHPBackGauge.fillAmount - DecreaseSpeed) > BossHPGauge.fillAmount)
            {
                BossHPBackGauge.fillAmount -= DecreaseSpeed;
            }
            else
            {
                BossHPBackGauge.fillAmount = BossHPGauge.fillAmount;
            }
            yield return new WaitForEndOfFrame();
        }
        HPdecreasing = false;
    }
}
