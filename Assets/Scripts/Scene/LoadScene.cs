using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    GameObject backGroundImage;

    [SerializeField]
    Image loadSceneTextBox;
    [SerializeField]
    Sprite[] loadSceneText;

    [SerializeField]
    GameObject BackGround;
    [SerializeField]
    GameObject[] loadSceneImage;
    GameObject obj;


    [SerializeField]
    LoadFadeImage fade;
    string loadScene;

    [SerializeField, Header("プレイヤー残機表示Image")]
    Image stockImg;
    [SerializeField, Header("残機画像")]
    Sprite[] stockImgs;

    //ロードシーン待機時間
    [SerializeField]
    [Header("ロード待機時間")]
    public float time = 2f;

    //多重発生防止bool
    bool canLoadScene = true;

    [SerializeField, Header("チュートリアルスキップ画像")]
    GameObject tutorialSkipObj;
    [SerializeField, Header("スキップゲージ")]
    Image skipGage;
    [SerializeField, Header("スキップに必要な入力時間")]
    float skipTime = 1f;
    float inputTime = 0;
    
    //InputSystem
    internal InputAction decision;

    private void Start()
    {
        System.GC.Collect();
        Cursor.visible = false;
        loadSceneTextBox.sprite = null;
        switch (SceneData.Instance.referer)
        {
            case "Title":
                obj = (GameObject)Instantiate(loadSceneImage[0], BackGround.transform.position, Quaternion.identity);
                obj.transform.parent = BackGround.transform;
                loadSceneTextBox.sprite = loadSceneText[0];
                loadScene = "Level_Tutorial";
                //チュートリアルスキップ画像を表示
                time = 2f;
                tutorialSkipObj.SetActive(true);
                var playerInput = GetComponent<PlayerInput>();
                decision = playerInput.actions["Decision"];
                break;
            case "Tutorial":
                obj = (GameObject)Instantiate(loadSceneImage[1], BackGround.transform.position, Quaternion.identity);
                obj.transform.parent = BackGround.transform;
                loadSceneTextBox.sprite = loadSceneText[1];
                loadScene = "Level_Stage1";
                break;
            case "Stage1":
                obj = (GameObject)Instantiate(loadSceneImage[2], BackGround.transform.position, Quaternion.identity);
                obj.transform.parent = BackGround.transform;
                loadSceneTextBox.sprite = loadSceneText[2];
                loadScene = "Level_Stage2";
                break;
            case "Stage2":
                obj = (GameObject)Instantiate(loadSceneImage[3], BackGround.transform.position, Quaternion.identity);
                obj.transform.parent = BackGround.transform;
                loadSceneTextBox.sprite = loadSceneText[3];
                loadScene = "Stage3";
                break;
        }

        stockImg.sprite = stockImgs[SceneData.Instance.stock];
    }

    // Start is called before the first frame update

    private void Update()
    {
        if (fade.IsFadeInComplete() && canLoadScene)
        {
            canLoadScene = false;
            StartCoroutine(LoadStart());
        }
        if (SceneData.Instance.referer == "Title")
        {
            if (decision.IsPressed() && inputTime < skipTime)
            {
                inputTime += Time.deltaTime;
                skipGage.fillAmount = inputTime / skipTime;
            }
        }
    }

    private void FixedUpdate()
    {
        var posX = backGroundImage.transform.position.x - 0.2f;
        backGroundImage.transform.position = new Vector2(posX, backGroundImage.transform.position.y);
    }

    IEnumerator LoadStart()
    {
        yield return new WaitForSeconds(time);
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        System.GC.Collect();
        Resources.UnloadUnusedAssets();

        SceneData.Instance.revival = false;
        SceneData.Instance.wayPoint_1 = false;
        SceneData.Instance.wayPoint_2 = false;
        SceneData.Instance.playTime = 0;

        //スキップに必要な時間分入力されていればチュートリアルスキップ
        if (inputTime > skipTime)
        {
            loadScene = "Load";
            SceneData.Instance.referer = "Tutorial";
        }

        SceneManager.LoadScene(loadScene);
    }
}
