using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Result : MonoBehaviour
{

    [SerializeField] FadeImage fade;
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

    [SerializeField]
    Canvas canvas;

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
        public TextMeshProUGUI crearTime_Bar;
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
        canvas.enabled = false;
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

        //�N���A�^�C��������
        if (numList.crearTime_Bar != null)
        {
            numList.crearTime_Bar.text = "";
        }
    }

    public void Result_Set(int clearStageID, int score, int combo, int killScore)
    {
        //�����G���f�B���O�p
        if (clearStageID == 3) {
            StartCoroutine(Ending());
            return;
        }

        foreach (ClearStageList clearStage in clearStageList)
        {
            if (clearStage.CleatStage_ID == clearStageID)
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
        //�v���C���ԕ\��
        if (numList.crearTime_Bar != null)
        {
            numList.crearTime_Bar.text = getTimeString(SceneData.Instance.playTime);
        }

        var clearScore = score + combo + killScore;

        if (clearScore >= RANK_S)
        {
            RankBox.sprite = RankImageList[0];
        }
        else if (clearScore >= RANK_A)
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
        canvas.enabled = true;
        StartCoroutine(AnyKey());
    }

    IEnumerator AnyKey()
    {
        yield return new WaitForSeconds(5f);
        anyKay.enabled = true;
        PreesAnyKey.enabled = true;
        canAnyKey = true;
    }

    IEnumerator Ending()
    {
        yield return new WaitForSeconds(1);//�n���ꂽ���ԑҋ@
        //�t�F�[�h�A�E�g�J�n
        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        SceneManager.LoadScene("Ending");
    }

    //�v���C���ԕ��͉�
    string getTimeString(float time)
    {
        int sec = (int)time;
        int mm = sec / 60;
        int ss = sec % 60;
        return timeCher(mm.ToString("D2")) + "<sprite=11>" + timeCher(ss.ToString("D2"));

        string timeCher(string str)
        {
            string returnString = "";
            var c = str.ToCharArray();

            for (int i = 0; i < c.Length; i++)
            {
                returnString += "<sprite=" + c[i] + ">";
            }

            return returnString;
        }
    }
}
