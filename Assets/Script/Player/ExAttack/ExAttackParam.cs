using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExAttackParam : MonoBehaviour
{
    [Header("ƒvƒŒƒCƒ„[")]
    [SerializeField]
    PlayerController player;

    [Header("ƒQ[ƒW—Ê")]
    [SerializeField]
    int gauge;

    //TODO@C³—\’è
    [Header("ƒQ[ƒW‚Ì‰æ‘œ")]
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

    //•KE‹Z‚ğ”­“®‚µ‚½ÛŒÄ‚Ô
    public void _EXAttack()
    {
        exAttackText.GetComponent<Image>().enabled = false;
        exGauge.fillAmount = 0f;
        _exAttack = 0;
        isExAttack = false;
        
    }

    public void AddGauge()
    {
        if (_exAttack > gauge) { return; };

        exGauge.fillAmount += 1f / gauge;
        _exAttack++;

        if (gauge == _exAttack)
        {
            exAttackText.GetComponent<Image>().enabled = true;
            isExAttack = true;
        }
    }

    internal int GetGage()
    {
        return _exAttack;
    }

    internal void SetGage(int backSceneExGage)
    {
        for(int i = 0;i < backSceneExGage; i++)
        {
            AddGauge();
        } 

    }
}
