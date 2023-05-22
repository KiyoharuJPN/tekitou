using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExAttackParam : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField]
    PlayerController player;

    [Header("ゲージ量")]
    [SerializeField]
    int gauge;

    //TODO　修正予定
    [Header("ゲージの画像")]
    [SerializeField]
    Image exGauge;
    [SerializeField]
    GameObject exAttackText;

    private bool isExAttack;

    public bool GetIsExAttack
    {
        get { return isExAttack; }
    }

    int _exAttack;

    public static ExAttackParam Instance { get; private set; }

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

    void Start()
    {
        exAttackText.GetComponent<Image>().enabled = false;
        exGauge.fillAmount = 0f;
    }

    //必殺技を発動した際呼ぶ
    public void _EXAttack()
    {
        exAttackText.GetComponent<Image>().enabled = false;
        exGauge.fillAmount = 0f;
        _exAttack = 0;
        isExAttack = false;
        
    }

    public void AddGauge()
    {
        //exGauge.fillAmount += 1f / gauge;
        //_exAttack++;

        //デバッグ用
        exGauge.fillAmount = 1f;
        _exAttack += 80;

        if (gauge == _exAttack)
        {
            exAttackText.GetComponent<Image>().enabled = true;
            isExAttack = true;
        }

        
    }
}
