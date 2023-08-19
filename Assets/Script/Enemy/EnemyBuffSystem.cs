using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyBuffSystem : MonoBehaviour
{
    [SerializeField, Tooltip("Buffを獲得するまでの必要攻撃数")]
    int BuffAttackCheck = 3;
    

    TextMeshProUGUI BuffAttackCheckText;
    GameObject BuffCanvas;

    //表示時(暫定)
    public enum DisplayType
    {
        EnemyLive,
        EnemyDead,
        Alltime,
    }

    //バフ種類
    public enum SetBuffType
    {
        HeroExSkillGaugeUp,
        HeroSpeedUp,
        HeroSlashingBuff,
        HeroinvincibleBuff,
        NoBuff,
        RandomSet,
    }

    //Enemy enemy;
    //public DisplayType displayType = DisplayType.Alltime;
    public Vector3 intervalPos;
    public SetBuffType buffType = SetBuffType.NoBuff;
    public GameObject[] DeadEffect;
    public GameObject TextObject;

    private void Start()
    {
        if(buffType == SetBuffType.RandomSet)
        {
            var newbuffType = (int)Random.Range(0, (float)SetBuffType.NoBuff);
            buffType = (SetBuffType)newbuffType;
        }
        BuffCanvas = GameObject.Find("BuffCanvas");
        BuffAttackCheckText = Instantiate(TextObject,BuffCanvas.transform).GetComponent<TextMeshProUGUI>();
        BuffAttackCheckText.gameObject.SetActive(false);
        //enemy = GetComponentInParent<Enemy>();
    }

    private void Update()
    {
        if (BuffAttackCheckText.gameObject.activeSelf)
        {
            BuffAttackCheckText.gameObject.transform.position = transform.position + intervalPos;
        }
    }

    //最初に表示されるアタック必要数のセット
    public void SetBuffAttackCheckCount(int count)
    {
        BuffAttackCheck = count;
    }
    //アタック必要数のゲット関数
    public int GetBuffAttackCheckCount()
    {
        return BuffAttackCheck;
    }

    //現在残りのアタック必要数表示
    public void ShowAttackChecking()
    {
        //倒された時は表示だけをする
        if (!BuffAttackCheckText.gameObject.activeSelf)
        {
            BuffAttackCheckText.color = GetColorByType();
            BuffAttackCheckText.text = "" + BuffAttackCheck-- + "";
            BuffAttackCheckText.gameObject.SetActive(true);
            return;
        }

        //カウントを減らして表示する
        BuffAttackCheckText.text = "" + BuffAttackCheck-- + "";
        if(BuffAttackCheck < 0) _Destroy();

    }


    //BuffTypeを外から取得
    public SetBuffType GetBuffType()
    {
        return buffType;
    }


    //Buff対応のエフェクトを外から取得
    public GameObject GetBuffEffect()
    {
        return DeadEffect[(int)buffType];
    }

    //Buff対応の色を外から取得
    public Color GetColorByType(/*SetBuffType type*/)
    {
        Color color = new Color(0,0,0,0);
        switch (buffType)
        {
            case SetBuffType.HeroExSkillGaugeUp:
                color = HeroExSkillGaugeUpOrange();
                break;
            case SetBuffType.HeroSpeedUp:
                color = HeroSpeedUpBlue();
                break;
            case SetBuffType.HeroSlashingBuff:
                color = HeroSlashingBuffGreen();
                break;
            case SetBuffType.HeroinvincibleBuff:
                color = HeroinvincibleBuffYello();
                break;
            case SetBuffType.NoBuff:
            default:
                break;
        }
        return color;
    }


    //Buff色設定
    Color HeroExSkillGaugeUpOrange()
    {
        //return new Color(243, 152, 0, 255);
        return new Color(0.95f, 0.53f, 0, 1);
    }
    Color HeroSpeedUpBlue()
    {
        //return new Color(0, 0, 255, 255);
        return new Color(0, 0, 1, 1);
    }
    Color HeroSlashingBuffGreen()
    {
        //return new Color(0, 255, 0, 255);
        return new Color(0, 1, 0, 1);
    }
    Color HeroinvincibleBuffYello()
    {
        //return new Color(255, 255, 0, 255);
        return new Color(1, 1, 0, 1);
    }

    public void _Destroy()
    {
        Destroy(BuffAttackCheckText.gameObject);
    }
}
