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
        [Tooltip("HP����")]
        public int FullHP;
        [Tooltip("��ʂ̉�����n�[�g�܂ł̋���"), Range(0, 1)]
        public float HeartHight;
        [Tooltip("��ʂ̍�����n�[�g�܂ł̋���"), Range(0, 0.1f)]
        public float HeartWidth;
        [Tooltip("��ʂ̍�����n�[�g�܂ł̋���"), Range(0, 0.1f)]
        public float HeartLeftJustified;
        //���JStatus�ǉ��p
    }
    [SerializeField]
    [Header("HP�X�e�[�^�X")]
    HPStatus HPstatus = new HPStatus { FullHP = 10, HeartHight = 0.7f, HeartWidth = 0.04f, HeartLeftJustified = 0.03f};

    private void Start()
    {
        heals = HPstatus.FullHP;
        hp_preb = HPstatus.FullHP;
        heart = new GameObject[HPstatus.FullHP];//��Ԋm��
        var parent = this.transform;
        for (int i = 0; i < HPstatus.FullHP / 2; i++)
        {
            Instantiate(hearts[2], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);//�󗓃n�[�g�̍쐬
            heart[i * 2] = Instantiate(hearts[1], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);//�n�[�t�n�[�g�̍쐬
            if (i == 0) heart[1] = Instantiate(hearts[0], new Vector3((i * Screen.width * HPstatus.HeartWidth) + Screen.width * HPstatus.HeartLeftJustified, Screen.height * HPstatus.HeartHight, 0), Quaternion.identity, parent);//�n�[�g�̍쐬
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


    //HP��get&set�֐�
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
