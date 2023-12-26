using TMPro;
using UnityEngine;

public class EnemyBuffSystem : MonoBehaviour
{
    [SerializeField, Tooltip("基本追撃回数")]
    int initialBuffAttackCheck = 3;
    int BuffAttackCheck　= 0;

    //表示オブジェクトと置きキャンバス
    SpriteRenderer BuffAttackCheckText;
    GameObject BuffCanvas;
    //ぶっ飛び加速度とゲーム中バフ修正できるためのブール関数
    bool checkBlowingUp = false, canSetBuffType = true;

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
    [SerializeField, Tooltip("追撃表示イラスト")]
    Sprite[] sprites;

    //表示時高さオフセット、DefaultBuff、死亡時エフェクト、TextPrefabとCanvasPrefab
    public Vector3 intervalPos;
    public SetBuffType buffType = SetBuffType.NoBuff;
    public GameObject[] DeadEffect;
    public GameObject TextObject, CanvasObject;

    ////表示時(暫定)
    //public enum DisplayType
    //{
    //    EnemyLive,
    //    EnemyDead,
    //    Alltime,
    //}
    //Enemy enemy;
    //public DisplayType displayType = DisplayType.Alltime;

    private void Awake()
    {
        if (buffType == SetBuffType.RandomSet)
        {
            var newbuffType = (int)Random.Range(0, (float)SetBuffType.NoBuff);
            buffType = (SetBuffType)newbuffType;
        }
        if(buffType == SetBuffType.NoBuff)
        {
            Destroy(GetComponent<EnemyBuffSystem>());
            return;
        }
        if (GameObject.Find("BuffCanvas"))
        {
            BuffCanvas = GameObject.Find("BuffCanvas");
        }
        else
        {
            BuffCanvas = Instantiate(CanvasObject);
            BuffCanvas.name = "BuffCanvas";
        }
        BuffAttackCheckText = Instantiate(TextObject,BuffCanvas.transform).GetComponent<SpriteRenderer>();
        BuffAttackCheckText.gameObject.SetActive(false);
        BuffAttackCheck = initialBuffAttackCheck;
        //enemy = GetComponentInParent<Enemy>();
    }

    private void Update()
    {
        if (BuffAttackCheckText != null && BuffAttackCheckText.gameObject.activeSelf)
        {
            BuffAttackCheckText.gameObject.transform.position = transform.position + intervalPos;
        }
    }

    //現在残りのアタック必要数表示
    public void ShowAttackChecking()
    {
        //倒された時は表示だけをする
        if (!BuffAttackCheckText.gameObject.activeSelf)
        {
            canSetBuffType = false;
            BuffAttackCheck = GetBuffAcquisitionCount() > 10 ? 10: GetBuffAcquisitionCount();
            if (BuffAttackCheck <= 0)
                Debug.Log("ゼロ以上の値を入れてください。");
            else
                BuffAttackCheckText.sprite = sprites[BuffAttackCheck-- - 1 + (int)buffType * 10];
            BuffAttackCheckText.gameObject.SetActive(true);
            return;
        }


        //カウントを減らして表示する
        if (BuffAttackCheck > 0)
            BuffAttackCheckText.sprite = sprites[BuffAttackCheck-- - 1 + (int)buffType * 10];
        else
            BuffAttackCheck--;
        if (BuffAttackCheck < 0)
        {
            if (!checkBlowingUp)
            {
                checkBlowingUp = true;
                var enemys = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var emy in enemys)
                {
                    if (emy.GetComponent<Enemy>() != null && emy.GetComponent<Enemy>().isDestroy)
                    {
                        emy.GetComponent<Enemy>().BuffBoostSphere();
                    }
                }
                checkBlowingUp = false;
            }
            _Destroy();
            return;
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

    //エクストラスキル当たられたときに追撃回数を指定回数減らす
    public void SetEXAttackDecrease(int exa)
    {
        //EXAttack = exa;
        BuffAttackCheck -= Mathf.Abs(exa);
    }
    //public bool GetEXAttack()
    //{
    //    return EXAttack;
    //}

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

    //取得回数を外で取得する
    public int GetBuffAcquisitionCount()
    {
        if (BuffAttackCheck == 0) BuffAttackCheck = initialBuffAttackCheck;
        int count = 0;
        switch (buffType)
        {
            case SetBuffType.HeroExSkillGaugeUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.ExGage);
                break;
            case SetBuffType.HeroSpeedUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp);
                break;
            case SetBuffType.HeroSlashingBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Slashing);
                break;
            case SetBuffType.HeroinvincibleBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible);
                break;
            default:
                count = 0;
                break;
        }
        return BuffAttackCheck + (count / 2);
    }

    //吹っ飛び速度を外で取得する
    public int GetBuffBlowingSpeed()
    {
        int count = 0;
        switch (buffType)
        {
            case SetBuffType.HeroExSkillGaugeUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.ExGage);
                break;
            case SetBuffType.HeroSpeedUp:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.SpeedUp);
                break;
            case SetBuffType.HeroSlashingBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Slashing);
                break;
            case SetBuffType.HeroinvincibleBuff:
                count = PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible);
                break;
            default:
                count = 0;
                break;
        }
        return count;
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

    //外部でバフ内容を決める
    public void SetBuffTypeByScript(int bt)
    {
        if (canSetBuffType) buffType = (SetBuffType)bt;
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

    //常に使うけど修正はない関数
    public void _Destroy()
    {
        //Destroy(BuffAttackCheckText.gameObject);
    }

    private void OnDisable()
    {
        if (BuffAttackCheckText!=null)
        {
            BuffAttackCheckText.gameObject.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        if (BuffAttackCheckText != null)
        {
            _Destroy();
        }
    }
}
