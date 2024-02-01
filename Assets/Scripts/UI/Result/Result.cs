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
    //クリアステージ表示
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

    //クリアランク
    [SerializeField]
    UnityEngine.UI.Image RankBox;
    [SerializeField]
    Sprite[] RankImageList;
    public int RANK_S = 7000;
    public int RANK_A = 3000;

    //各スコア
    [System.Serializable]
    public struct NumList
    {
        public TextMeshProUGUI ScoreBar;
        public TextMeshProUGUI Combo_Bar;
        public TextMeshProUGUI Kill_Bar;
        public TextMeshProUGUI crearTime_Bar;
    }
    [Header("スコア等TextMeshProリスト")]
    [SerializeField]
    public NumList numList;
    int point;

    //次へボタン
    [SerializeField]
    UnityEngine.UI.Image PreesAnyKey;
    private bool canAnyKey;
    ResultAnyKay anyKay;

    [SerializeField] private GameObject newTextObj;

    public bool getCanAnyKey { get { return canAnyKey; } }

    [System.Serializable]
    public struct TimeBonus
    {
        public float time;
        public int bonus;
    }
    public List<TimeBonus> timeBonusList = new();

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
        //クリアランク・ステージ名初期化
        ClearStage_NameBar.sprite = null;
        RankBox.sprite = null;

        string SpriteText;
        point = 0;
        //スコア初期化
        SpriteText = point.ToString("d7");
        numList.ScoreBar.text = "";
        foreach (var i in SpriteText)
        {
            numList.ScoreBar.text += "<sprite=" + i + ">";
        }
        //Maxコンボ初期化
        SpriteText = point.ToString("d3");
        numList.Combo_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Combo_Bar.text += "<sprite=" + i + ">";
        }
        //敵を倒した数初期化
        SpriteText = point.ToString("d4");
        numList.Kill_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Kill_Bar.text += "<sprite=" + i + ">";
        }

        //クリアタイム初期化
        if (numList.crearTime_Bar != null)
        {
            numList.crearTime_Bar.text = "";
        }
    }

    public void Result_Set(int clearStageID, int score, int combo, int killScore)
    {

        foreach (ClearStageList clearStage in clearStageList)
        {
            if (clearStage.CleatStage_ID == clearStageID)
            {
                ClearStage_NameBar.sprite = clearStage.ClearStage_Image;
            }
        }

        string SpriteText;

        //スコアセット
        point = score;
        SpriteText = point.ToString("d7");
        numList.ScoreBar.text = "";
        foreach (var i in SpriteText)
        {
            numList.ScoreBar.text += "<sprite=" + i + ">";
        }

        //Maxコンボ初期化
        point = combo;
        SpriteText = point.ToString("d3");
        numList.Combo_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Combo_Bar.text += "<sprite=" + i + ">";
        }

        //敵を倒した数初期化
        point = killScore;
        SpriteText = point.ToString("d4");
        numList.Kill_Bar.text = "";
        foreach (var i in SpriteText)
        {
            numList.Kill_Bar.text += "<sprite=" + i + ">";
        }
        //プレイ時間表示
        if (numList.crearTime_Bar != null)
        {

            var playTime = SceneData.Instance.playTime;
            var isNewTime = SceneData.Instance.NewPlayTimeCheck(clearStageID, playTime);

            if (isNewTime)
            {
                newTextObj.SetActive(true);
            }
            else if(!isNewTime)
            {
                newTextObj.SetActive(false);
            }

            numList.crearTime_Bar.text = getTimeString(playTime);

            switch (clearStageID)
            {
                case 1:
                    SceneData.Instance.PlayTimeSeve(Gamepara.StageType.stage1);
                    break;
                case 2:
                    SceneData.Instance.PlayTimeSeve(Gamepara.StageType.stage2);
                    break;
                case 3:
                    SceneData.Instance.PlayTimeSeve(Gamepara.StageType.stage3);
                    break;
            }
        }

        var clearScore = score + combo * 10 + killScore * 100 + GetTimeBonus();

        if (clearScore >= RANK_S)
        {
            RankBox.sprite = RankImageList[0];
            SceneData.Instance.SetClearRank(clearStageID, Gamepara.ClearRank.S); 
        }
        else if (clearScore >= RANK_A)
        {
            RankBox.sprite = RankImageList[1];
            SceneData.Instance.SetClearRank(clearStageID, Gamepara.ClearRank.A);
        }
        else
        {
            RankBox.sprite = RankImageList[2];
            SceneData.Instance.SetClearRank(clearStageID, Gamepara.ClearRank.B);
        }

        //オールS実績確認
        if (SceneData.Instance.ClearRankCheck())
            Accmplisment.Instance.AchvOpen("PlayLank");
    }

    public void Result_Start()
    {
        canvas.enabled = true;
        switch (SceneData.Instance.referer)
        {
            case "Tutorial":
                SceneData.Instance.StageOpen(Gamepara.StageType.Tutorial);
                break;

            case "Stage1":
                SceneData.Instance.StageOpen(Gamepara.StageType.stage1);
                break;

            case "Stage2":
                SceneData.Instance.StageOpen(Gamepara.StageType.stage2);
                break;

            case "Stage3":
                SceneData.Instance.StageOpen(Gamepara.StageType.stage3);
                break;
        }
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
        yield return new WaitForSeconds(1);//渡された時間待機
        //フェードアウト開始
        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        SceneManager.LoadScene("Ending");
    }

    //プレイ時間文章化
    string getTimeString(float time)
    {
        int sec = (int)time;
        int mm = sec / 60;
        int ss = sec % 60;
        return timeCher(mm.ToString("D2")) + "<sprite=10>" + timeCher(ss.ToString("D2"));

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

    int GetTimeBonus()
    {
        var playTime = SceneData.Instance.playTime;
        if(timeBonusList.Count == 0)
        {
            return 0;
        }
        for(int i = 0; i < timeBonusList.Count; i++)
        {
            if (playTime < timeBonusList[i].time)
            {
                return timeBonusList[i].bonus;
            }
        }

        return 0;
    }
}
