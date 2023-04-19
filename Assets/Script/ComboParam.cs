using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ComboParam : MonoBehaviour
{
    [Tooltip("表示用テキストボックス")]
    public Text text;   //リアルタイム更新用テキストボックス
    private int countCombo, CCb_preb;
    private float time;

    [System.Serializable]
    struct ComboStatus
    {
        [Tooltip("Combo消滅時間")]
        public int Distime;
        //公開Status追加用
    }
    [SerializeField]
    [Header("Comboステータス")]
    ComboStatus comboStatus = new ComboStatus { Distime = 3 };

    // Start is called before the first frame update
    void Start()
    {
        countCombo = 0;
        CCb_preb = 0;//更新判定用
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (countCombo <= 0) countCombo = 0;//コンボ最小限
        if (countCombo >= 999) countCombo = 999;//コンボ最大限
        //if (countCombo == 0) text.gameObject.SetActive(false);//0の時に画面から消す

        if (countCombo != 0 && countCombo != CCb_preb || countCombo == 1)//表示コード
        {
            //if (!text.gameObject.activeSelf) text.gameObject.SetActive(true);
            CCb_preb = countCombo;
            text.text = "<size=25>X</size><size=50> " + countCombo + " </size><size=30>COMBO</size>";
            time = 0;
        }

        //Debug.Log("time"+time +"distime"+comboStatus.Distime);
        time += Time.deltaTime;//時間経過の計算
        if (time > comboStatus.Distime)//一定時間が立つとゼロに戻す
        {
            countCombo = 0;
        }
    }




    //ゲットセット関数
    public int GetCombo()
    {
        return countCombo;
    }
    public void SetCombo(int Cb)
    {
        countCombo = Cb;
    }

    public float GetPowerUp()
    {
        return (float)countCombo * (float)0.05;
    }
}
