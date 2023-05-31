using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Result : MonoBehaviour
{
    //�N���A�X�e�[�W�\��
    [SerializeField]
    UnityEngine.UI.Image ClearStage_NameBar;
    [System.Serializable]
    public struct ClearStageList
    {
        public int CleatStage_ID;
        public Sprite ClearStage_Image;
    }
    [SerializeField]
    public List<ClearStageList> clearStageList = new List<ClearStageList>();

    //�N���A�����N
    [SerializeField]
    UnityEngine.UI.Image RankBox;
    [SerializeField]
    Sprite[] RankImageList;
    const int RANK_S = 7000;
    const int RANK_A = 3000;

    //�e�X�R�A
    [System.Serializable]
    public struct NumList
    {
        public TextMeshProUGUI ScoreBar;
        public TextMeshProUGUI Combo_Bar;
        public TextMeshProUGUI Kill_Bar;
    }
    [Header("�X�R�A��TextMeshPro���X�g")]
    [SerializeField]
    public NumList numList;
    int point;

    //���փ{�^��
    [SerializeField]
    UnityEngine.UI.Image PreesAnyKey;
    private bool canAnyKey;
    ResultAnyKay anyKay;

    public bool getCanAnyKey { get { return canAnyKey; } }

    public static Result Instance { get; private set; }

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
        anyKay = this.GetComponent<ResultAnyKay>();
        this.GetComponent<Canvas>().enabled = false;
        //�N���A�����N�E�X�e�[�W��������
        ClearStage_NameBar.sprite = null;
        RankBox.sprite = null;

        string SpriteText;
        point = 0;
        //�X�R�A������
        SpriteText = point.ToString("d7");
        numList.ScoreBar.text = "";
        foreach (var i in SpriteText)
        {    
            numList.ScoreBar.text += "<sprite=" + i + ">";
        }
        //Max�R���{������
        SpriteText = point.ToString("d3");
        numList.Combo_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Combo_Bar.text += "<sprite=" + i + ">";
        }
        //�G��|������������
        SpriteText = point.ToString("d4");
        numList.Kill_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Kill_Bar.text += "<sprite=" + i + ">";
        }
    }

    public void Result_Set(int clearStageID,int score, int combo, int killScore)
    {
        foreach(ClearStageList clearStage in clearStageList)
        {
            if(clearStage.CleatStage_ID == clearStageID)
            {
                ClearStage_NameBar.sprite = clearStage.ClearStage_Image;
            }
        }

        string SpriteText;

        //�X�R�A�Z�b�g
        point = score;
        SpriteText = point.ToString("d7");
        numList.ScoreBar.text = "";
        foreach (var i in SpriteText)
        {
            numList.ScoreBar.text += "<sprite=" + i + ">";
        }

        //Max�R���{������
        point = combo;
        SpriteText = point.ToString("d3");
        numList.Combo_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Combo_Bar.text += "<sprite=" + i + ">";
        }

        //�G��|������������
        point = killScore;
        SpriteText = point.ToString("d4");
        numList.Kill_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Kill_Bar.text += "<sprite=" + i + ">";
        }

        var clearScore = score + combo + killScore;

        if(clearScore >= RANK_S)
        {
            RankBox.sprite = RankImageList[0];
        }
        else if(clearScore >= RANK_A) 
        {
            RankBox.sprite = RankImageList[1];
        }
        else
        {
            RankBox.sprite = RankImageList[2];
        }
    }

    public void Result_Start()
    {
        this.GetComponent<Canvas>().enabled = true;
        StartCoroutine(AnyKey());
    }

    IEnumerator AnyKey()
    {
        yield return new WaitForSeconds(5f);
        SceneData.Instance.ExGage =  ExAttackParam.Instance.GetGage();
        anyKay.enabled = true;
        PreesAnyKey.enabled = true;
        canAnyKey = true;
    }
}
