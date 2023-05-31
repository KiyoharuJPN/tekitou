using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinishMenu : MonoBehaviour
{
    [Tooltip("目標のシーンネームを書いてください")]
    public string SceneName;

    [System.Serializable]
    struct FadeOutOption
    {
        [Tooltip("フェードアウトの開始時間")]
        public float waitSecondTry, wateSecondTitle;
        [Tooltip("フェードアウトの持続時間")]
        public float fadeOutSpeedTry, fadeOutSpeedTitle;
    }
    public float mouseMoveWait = 1f;

    //ターゲット
    public GameObject target, fadeOut;
    public GameObject[] Finishobj;
    public Animator animator;
    

    bool pointerCheck = true, canChangePointer = true, canChoose = true;
    bool isRetry = false, isBack = false;
    float animWait,animSpeed;

    //float timeCount;
    //ポインター
    int pointer = 0, pointerpreb = -1;


    [SerializeField]
    [Header("フェードアウト設定")]
    FadeOutOption fadeOutOption = new() { waitSecondTry = 1f, wateSecondTitle = 1f, fadeOutSpeedTry = 10f, fadeOutSpeedTitle = 10f};

    private void Start()
    {
        Cursor.visible = false;
        SoundManager.Instance.PlayBGM(BGMSoundData.BGM.GameOver_intro, BGMSoundData.BGM.GameOver_roop);
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("joystick button 7"))
        {
            SceneManager.LoadScene("Title");
        }
        //調整キーの設定
        if (canChangePointer)
        {
            ChangePointer();
        }

        //ポインターが変わった時の設定
        PointerHasChange();

        //選択キーの指定
        if (canChoose) Choose();

        animator.SetBool("IsRetry", isRetry);
        animator.SetBool("IsBack", isBack);

        //Debug.Log(Input.GetAxis("Vertical")+"MOUSEIN"+pointerCheck);
    }

    
    //実行項目
    void TryAgain()
    {
        //Debug.Log(SystemController.Instance.GetLastScene());
        if (SceneData.Instance.referer == "Tutorial")
        {
            SceneManager.LoadScene("Level_Tutorial");
        }
        else if (SceneData.Instance.referer == "Stage1")
        {
            SceneManager.LoadScene("Level_Stage1");
        }
    }
    void BackTitle()
    {
        if (SceneName != "") SceneManager.LoadScene("Title");
    }
    /*IEnumerator PointerMoveWait()
    {
        yield return new WaitForSecondsRealtime(mouseMoveWait);
        pointerCheck = true;
        //Debug.Log("=========================================================================================");
    }*/
    IEnumerator Wait(int pointer, float waitSecond, float fadeOutSpeed)
    {
        OnSelected(Finishobj[pointer]);
        for (float i = waitSecond; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
        }
        for (float i = 0; i <= 1; i += fadeOutSpeed)
        {
            fadeOut.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null ;
        }
        Debug.Log(pointer);

        switch (pointer)
        {
            case 0:
                TryAgain();
                break;
            case 1:
                Debug.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");

                BackTitle();
                break;
            default:
                Debug.Log("新しい項目の追加はプログラマに頼んでください。");
                break;
        }
    }


    //調整キーの設定
    void ChangePointer()
    {
        /*if (Input.GetAxis("Vertical") > 0 && Input.GetAxis("Vertical") < vertical) vertical = 0;
        if (Input.GetAxis("Vertical") < 0 && Input.GetAxis("Vertical") > vertical) vertical = 0;*/
        if (Input.GetAxis("Vertical") > 0 && pointerCheck)
        {
            //StartCoroutine(PointerMoveWait());
            pointer--;
            pointerCheck = false;
        }
        if (Input.GetAxis("Vertical") < 0 && pointerCheck)
        {
            //StartCoroutine(PointerMoveWait());
            pointer++;
            pointerCheck = false;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            pointerCheck = true;
        }
    }
    //ポインターが変わった時の設定
    void PointerHasChange()
    {
        if (pointer != pointerpreb)
        {
            if (pointer < 0) pointer = 0;// Finishobj.Length - 1;
            if (pointer > Finishobj.Length - 1) pointer = Finishobj.Length - 1;//0;

            target.transform.position = new Vector2(target.transform.position.x, Finishobj[pointer].transform.position.y);

            //OnSelected(Finishobj[pointer]);
            //Debug.Log("pointer" + pointer + '\n' + "pointerpreb" + pointerpreb);
            //if (pointer != pointerpreb && pointerpreb != -1) OnDeselected(Finishobj[pointerpreb]);

            //ポインターを代入する
            pointerpreb = pointer;
        }
    }
    //選択キーの指定
    void Choose()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0"))
        {
            switch (pointer)
            {
                case 0:
                    SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_CutIn);
                    isRetry = true;
                    animWait = fadeOutOption.waitSecondTry;
                    animSpeed = fadeOutOption.fadeOutSpeedTry;
                    break;
                case 1:
                    isBack = true;
                    animWait = fadeOutOption.wateSecondTitle;
                    animSpeed = fadeOutOption.fadeOutSpeedTitle;
                    break;
                default:
                    Debug.Log("新しい項目の追加はプログラマに頼んでください。");
                    break;
            }
            StartCoroutine(Wait(pointer, animWait, animSpeed));
            canChangePointer = false;
            canChoose = false;
        }
    }
    
    //内部動き
    void OnSelected(GameObject obj)
    {
        obj.GetComponentInChildren<Text>().color = Color.yellow;             //UIの色修正
    }
/*    void OnDeselected(GameObject obj)
    {
        obj.GetComponentInChildren<Text>().color = new Color(255, 255, 255); //色を戻す
    }*/
}
