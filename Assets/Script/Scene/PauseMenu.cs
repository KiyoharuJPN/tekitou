using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PauseMenu : MonoBehaviour
{
    [Tooltip("今の選択を示すポインターです"), Header("トライアングルポインター")]
    public GameObject target;

    public GameObject[] menuobj;            //メニュー画面のオブジェクト
    //残り残機画像
    public Image stockImage;
    public Sprite[] stockImages;
    

    //メニュー表示確認Bool
    private bool isPauseMenu = false;

    public GameObject menuTextObj;
    private bool isMenuText = true;
    public GameObject actionExpoObj;
    private bool isActionExpo = false;

    [SerializeField]
    Animator player;

    [SerializeField]
    LoadFadeImage fade;

    bool canStart = true;

    //ポインターと一個前のポインター
    int pointer;
    int pointerpreb;

    //各種チェック用関数
    bool volumeChecking = false, inlineVolumeChecking = false, hideKeyChecking = false, pointerCheck = true, upDownLock = false;

    //InputSystem
    internal InputAction back, decision, move;

    private void Start()
    {
        pointer = 0;            //ポインターの初期化

        var playerInput = GameManager.Instance.playerInput;
        back = playerInput.actions["Back"];
        decision = playerInput.actions["Decision"]; 
        move = playerInput.actions["Move"];
    }

    public bool PauseCheck()
    {
        if (isPauseMenu) { return true; }
        return false;
    }

    public void PauseStart()
    {
        Time.timeScale = 0;
        isPauseMenu = true;
        stockImage.sprite = stockImages[SceneData.Instance.stock];
        this.GetComponent<Canvas>().enabled = true;
    }

    public void MenuUpdata()
    {
        //調整キーの設定
        if (!upDownLock) StickerChangePointer();

        //ポインターが変わった時の設定
        if (pointer != pointerpreb)//変更されたときの作業
        {
            if (menuobj[0].activeSelf)//Menu
            {
                if (pointer < 0) pointer = 0;// menuobj.Length - 1;
                if (pointer > menuobj.Length - 1) pointer = menuobj.Length - 1;// 0;//上限調整

                target.transform.position = new Vector2(target.transform.position.x, menuobj[pointer].transform.position.y);
                //OnSelected(menuobj[pointer]);
                //if(pointer !=pointerpreb && pointerpreb != -1) OnDeselected(menuobj[pointerpreb]);
            }

            //ポインターの修正
            pointerpreb = pointer;
        }

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
            SelectMenu();
        }

        //戻るキーの設定
        if (back.WasPressedThisFrame())
        {
            BackMenu();
        }
    }

    private void SelectMenu()
    {
        if (menuobj[0].activeSelf && !hideKeyChecking && isMenuText)//Menu
        {
            switch (pointer)
            {
                case 0:
                    BackGame();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 1:
                    ActionExpo();
                    //OnDeselected(menuobj[pointer]);
                    break;
                case 2:
                    Exit();
                    break;
                default:
                    if (pointer < menuobj.Length - 1)
                    {
                        //OnDeselected(menuobj[pointer]);
                        Debug.Log("新しい項目を追加するときはプログラマに頼んでください。");
                    }
                    break;
            }
            hideKeyChecking = true;
        }

        //ポインターの復元
        if (!volumeChecking && !inlineVolumeChecking)
        {
            pointer = 0;
            pointerpreb = -1;
        }
        inlineVolumeChecking = false;
        hideKeyChecking = false;
    }

    public void BackGame()
    {
        actionExpoObj.SetActive(false);
        menuTextObj.SetActive(true);
        isMenuText = true;
        isActionExpo = false;
        isPauseMenu = false;
        this.GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1;
        GameManager.Instance.PauseBack();
    }

    //メニューに戻る
    public void BackMenu()
    {
        actionExpoObj.SetActive(false);
        menuTextObj.SetActive(true);
        isMenuText = true;
        isActionExpo = false;
    }

    //アクション詳細表示
    private void ActionExpo()
    {
        menuTextObj.SetActive(false);
        actionExpoObj.SetActive(true);
        isMenuText = false;
        isActionExpo = true;
    }

    //タイトルを戻る
    void Exit()
    {
        upDownLock = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }

    void StickerChangePointer()
    {
        var input = move.ReadValue<Vector2>().y;
        if (input > 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer--;
        }
        if (input < 0 && pointerCheck)
        {
            pointerCheck = false;
            pointer++;
        }
        if (input == 0)
        {
            pointerCheck = true;
        }
    }

    IEnumerator Scene_Start()
    {
        player.SetTrigger("Start");
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_CutIn);
        yield return new WaitForSeconds(2.4f);

        fade.StartFadeOut();
        SceneData.Instance.DataReset();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
    }

    //内部動き
    void OnSelected(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.grey;               //UIの色修正
    }
    void OnDeselected(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255); //色を戻す
    }

}
