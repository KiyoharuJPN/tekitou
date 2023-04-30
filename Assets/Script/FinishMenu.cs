using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinishMenu : MonoBehaviour
{
    [Tooltip("目標のシーンネームを書いてください")]
    public string SceneName;
    [Tooltip("次のシーンに移動するまでの時間")]
    //public float waitSecond = 1f;

    //ターゲット
    public GameObject target, fadeOut;
    public GameObject[] Finishobj;
    public Animator animator;

    bool pointerCheck = true, canChangePointer = true, canChoose = true;
    bool isRetry = false, isBack = false;
    float animWait;

    //ポインター
    int pointer = 0, pointerpreb = -1;

    
    // Update is called once per frame
    void Update()
    {
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
    }


    //実行項目
    void TryAgain()
    {
        //Debug.Log(SystemController.Instance.GetLastScene());
        SceneManager.LoadScene(SystemController.Instance.GetLastScene());
    }
    void BackTitle()
    {
        if (SceneName != "") SceneManager.LoadScene(SceneName);
    }
    IEnumerator PointerMoveWait()
    {
        yield return new WaitForSeconds(0.1f);
        pointerCheck = true;
    }
    IEnumerator Wait(int pointer, float waitSecond)
    {
        OnSelected(Finishobj[pointer]);
        for (float i = waitSecond; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
        }
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            fadeOut.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null ;
        }
        switch (pointer)
        {
            case 0:
                TryAgain();
                break;
            case 1:
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
        if (Input.GetAxis("Vertical") > 0 && pointerCheck)
        {
            PointerMoveWait();
            pointer--;
            pointerCheck = false;
        }
        if (Input.GetAxis("Vertical") < 0 && pointerCheck)
        {
            PointerMoveWait();
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
                    isRetry = true;
                    animWait = 1f;
                    break;
                case 1:
                    isBack = true;
                    animWait = 1f;
                    break;
                default:
                    Debug.Log("新しい項目の追加はプログラマに頼んでください。");
                    break;
            }
            StartCoroutine(Wait(pointer, animWait));
            canChangePointer = false;
            canChoose = false;
        }
    }
    
    //内部動き
    void OnSelected(GameObject obj)
    {
        obj.GetComponentInChildren<Text>().color = Color.yellow;             //UIの色修正
    }
    void OnDeselected(GameObject obj)
    {
        obj.GetComponentInChildren<Text>().color = new Color(255, 255, 255); //色を戻す
    }
}
