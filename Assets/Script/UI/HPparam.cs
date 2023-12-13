using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPparam : MonoBehaviour
{
    [SerializeField, Header("ハートPrefab")]
    GameObject HeartObj;
    [SerializeField]
    Image hpGage;
    private int heals;
    private int hp_preb;

    private const int OneHeartHp = 2;

    [System.Serializable]
    struct HPStatus
    {
        [Tooltip("HP上限")]
        public int FullHP;
        [Tooltip("最初のハートPos")]
        public Vector2 fastHeartPos;
        [Tooltip("ハート間隔")]
        public Vector3 heartSpace;
    }
    [SerializeField]
    [Header("HPステータス")]
    HPStatus HPstatus = new HPStatus { FullHP = 6};

    
    public GameObject[] heartList;
    private Image[] heartImageList;

    private void Awake()
    {
        heals = HPstatus.FullHP;
        heartImageList = new Image[heartList.Length];
        
        for (int i = 0; i < heartImageList.Length; i++)
        {
            heartImageList[i] = heartList[i].transform.Find("HP_Heart").GetComponent<Image>();
        }
    }

    private void Start()
    {
        heals = HPstatus.FullHP;
    }

    public int GetHP()
    {
        return heals;
    }

    public void SetHP(int hp)
    {
        heals = hp;
        if (heals < 0) { heals = 0; }
        if (heals > HPstatus.FullHP) { heals = HPstatus.FullHP; }
        SetHPBar();
    }

    public void SetHPBar()
    {
        var remainder = heals % OneHeartHp;
        var changeHeart = heals / OneHeartHp;

        for (int i = 0; i < heartImageList.Length; i++)
        {
            if (changeHeart > 0)
            {
                heartImageList[i].fillAmount = 1;
                changeHeart--;
            }
            else if(changeHeart == 0)
            {
                heartImageList[i].fillAmount = 0;
            }
        }

        if (remainder > 0)
        {
            heartImageList[heals / OneHeartHp].fillAmount = 0.5f;
        }
    }
}
