using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExAttackParam : MonoBehaviour
{
    [Header("ÉvÉåÉCÉÑÅ[")]
    [SerializeField]
    PlayerController player;

    [Header("ÉQÅ[ÉWó ")]
    [SerializeField]
    int gauge;

    //TODOÅ@èCê≥ó\íË
    [Header("ÉQÅ[ÉWÇÃâÊëú")]
    [SerializeField]
    Image exGauge;
    [SerializeField]
    Image exGaugeFrame;
    [SerializeField]
    Sprite exMaxGaugeFrame;
    [SerializeField]
    GameObject exAttackText;

    private Sprite exNomalFrame;

    private bool canExAttack;

    public bool GetCanExAttack
    {
        get { return canExAttack; }
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

    //ïKéEãZÇî≠ìÆÇµÇΩç€åƒÇ‘
    public void EXAttack()
    {
        exAttackText.GetComponent<Image>().enabled = false;
        exGauge.fillAmount = 0f;
        _exAttack = 0;
        canExAttack = false;
        exGaugeFrame.sprite = exNomalFrame;
    }

    public void AddGauge()
    {
        if (_exAttack >= gauge) return;
        exGauge.fillAmount += 1f / gauge;
        _exAttack++;

        if (gauge == _exAttack)
        {
            MaxGage();
        }
        player.CanExAttackCheck();
    }

    internal int GetGage()
    {
        return _exAttack;
    }

    internal void SetGage(int exGageNum)
    {
        if (_exAttack >= gauge) return;
        exGauge.fillAmount = 1f;
        _exAttack = exGageNum;

        if (gauge == _exAttack)
        {
            MaxGage();
        }
        player.CanExAttackCheck();
    }

    private void MaxGage()
    {
        exAttackText.GetComponent<Image>().enabled = true;
        canExAttack = true;
        exNomalFrame = exGaugeFrame.sprite;
        exGaugeFrame.sprite = exMaxGaugeFrame;
    }
}
