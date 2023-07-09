using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboParam : MonoBehaviour
{
    [Tooltip("表示用テキストボックス")]
    public TextMeshProUGUI text;   //リアルタイム更新用テキストボックス
    private int countCombo, CCb_preb;
    private float time;

    [System.Serializable]
    struct ComboStatus
    {
        [Header("Combo消滅時間")]
        public int Distime;
        //公開Status追加用
    }
    [SerializeField]
    [Header("Comboステータス")]
    ComboStatus comboStatus = new ComboStatus { Distime = 3 };

    [SerializeField]
    Image comboTimeGage;

    [SerializeField]
    PlayerController player;


    bool isCombo;
    bool comboStop = false;

    public static ComboParam Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        countCombo = 0;
        comboTimeGage.fillAmount = 0f;
        CCb_preb = 0;//更新判定用
        text.text = "<sprite=" + 0 + ">"; 
        time = 0;
    }

    //ゲットセット関数
    public int GetCombo()
    {
        return countCombo;
    }

    public void SetCombo(int Cb)
    {
        
        if (countCombo == 0)
        {
            StartCoroutine(_ComboTime());
        }

        countCombo = Cb;
        if (countCombo <= 0) countCombo = 0;//コンボ最小限
        if (countCombo >= 999) countCombo = 999;//コンボ最大限
        GameManager.Instance.AddMaxComobo(countCombo);

        string SpriteText = countCombo.ToString();
        text.text = "";
        foreach (var i in SpriteText){
            text.text += "<sprite=" + i + ">";
        }

        ResetTime();
    }

    public void ComboStop()
    {
        comboStop = true;
    }

    public void ComboResume()
    {
        comboStop = false;
    }

    //計測時間リセット
    public void ResetTime()
    {
        if (countCombo == 0) return;
        time = 0;
        comboTimeGage.fillAmount = 1.0f;
    }

    public float GetPowerUp()
    {
        return (float)countCombo * (float)0.05;
    }

    IEnumerator _ComboTime()
    {
        while (time < comboStatus.Distime)
        {
            if (player.isExAttack || player.isWarpDoor || !player.canMove || comboStop)
            {
                yield return null;
            }
            else
            {
                time += Time.deltaTime;
                comboTimeGage.fillAmount -= 1.0f / comboStatus.Distime * Time.deltaTime;
                yield return null;
            }
        }

        countCombo = 0;
        time = 0;
        string SpriteText = countCombo.ToString();
        text.text = "<sprite=" + SpriteText + ">";
    }
}
