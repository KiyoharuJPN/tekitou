using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPparam : MonoBehaviour
{
    public GameObject[] hearts;
    private int heals;
    private int hp_preb;
    private GameObject[] heart;

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
        hp_preb = HPstatus.FullHP;
        heart = new GameObject[HPstatus.FullHP];//空間確保
        var parent = this.transform;
        for (int i = 0; i < HPstatus.FullHP / 2; i++)
        {
            Instantiate(hearts[2], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);//空欄ハートの作成
            heart[i * 2] = Instantiate(hearts[1], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);//ハーフハートの作成
            if (i == 0) heart[1] = Instantiate(hearts[0], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);//ハートの作成
            if (i != 0) heart[i * 2 + 1] = Instantiate(hearts[0], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);
        }
    }

    private void Update()
    {
        if (heals > HPstatus.FullHP) { heals = HPstatus.FullHP; }
        if (heals < 0) { heals = 0; }
        if (heals != hp_preb)
        {
            for (int i = 0; i < heals; i++) { heart[i].SetActive(true); }
            for (int i = heals; i < HPstatus.FullHP; i++) { heart[i].SetActive(false); }
        }
        hp_preb = heals;

    }


    //HPのget&set関数
    public int GetHP()
    {
        return heals;
    }

    public void SetHP(int hp)
    {
        heals = hp;
    }

    public void DamageHP(int hp)
    {
        heals = hp;
    }

}
