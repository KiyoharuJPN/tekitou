using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPparam : MonoBehaviour
{
    [SerializeField]
    Image hpGage;
    private int heals;
    private int hp_preb;

    [System.Serializable]
    struct HPStatus
    {
        [Tooltip("HP総量")]
        public int FullHP;
        [Tooltip("画面の下からハートまでの距離"), Range(0, 1)]
        public float HeartHight;
        [Tooltip("画面の左からハートまでの距離"), Range(0, 0.1f)]
        public float HeartWidth;
        [Tooltip("画面の左からハートまでの距離"), Range(0, 0.1f)]
        public float HeartLeftJustified;
        //公開Status追加用
    }
    [SerializeField]
    [Header("HPステータス")]
    HPStatus HPstatus = new HPStatus { FullHP = 10, HeartHight = 0.7f, HeartWidth = 0.04f, HeartLeftJustified = 0.03f};

    private void Start()
    {
        heals = HPstatus.FullHP;
        hpGage.fillAmount = 1;
    }

    //HPのget&set関数
    public int GetHP()
    {
        return heals;
    }

    public void SetHP(int hp)
    {
        heals = hp;
        if (heals > HPstatus.FullHP) { heals = HPstatus.FullHP; }
        hpGage.fillAmount = (float) heals / HPstatus.FullHP;
    }

    public void DamageHP(int hp)
    {
        heals = hp;
        if (heals < 0) { heals = 0; }
        hpGage.fillAmount = (float) heals / HPstatus.FullHP;
    }

}
