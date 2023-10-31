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
    GameObject exAttackText;

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
    public void _EXAttack()
    {
        exAttackText.GetComponent<Image>().enabled = false;
        exGauge.fillAmount = 0f;
        _exAttack = 0;
        canExAttack = false;
    }

    public void AddGauge()
    {
        if (_exAttack >= gauge) return;
        exGauge.fillAmount += 1f / gauge;
        _exAttack++;

        if (gauge == _exAttack)
        {
            exAttackText.GetComponent<Image>().enabled = true;
            canExAttack = true;
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
            exAttackText.GetComponent<Image>().enabled = true;
            canExAttack = true;
        }
        player.CanExAttackCheck();
    }

    IEnumerator _SetGage(int i) 
    {
        for (int j = 0;j < i; j++)
        {
            if (_exAttack >= gauge) break;
            exGauge.fillAmount += 1f / gauge;
            _exAttack++;

            if (gauge == _exAttack)
            {
                exAttackText.GetComponent<Image>().enabled = true;
                canExAttack = true;
            }
            yield return null;
        }
    }
}
