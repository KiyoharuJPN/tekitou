using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    private void Start()
    {
        System.GC.Collect();
        Cursor.visible = false;
        loadSceneTextBox.sprite = null;
        if (SceneData.Instance.referer == "Title")
        {
            obj = (GameObject)Instantiate(loadSceneImage[0], BackGround.transform.position, Quaternion.identity);
            obj.transform.parent = BackGround.transform;
            loadSceneTextBox.sprite = loadSceneText[0];
            loadScene = "Level_Tutorial";
        }
        else if (SceneData.Instance.referer == "Tutorial")
        {
            obj = (GameObject)Instantiate(loadSceneImage[1], BackGround.transform.position, Quaternion.identity);
            obj.transform.parent = BackGround.transform;
            loadSceneTextBox.sprite = loadSceneText[1];
            loadScene = "Level_Stage1";
        }
        else if (SceneData.Instance.referer == "Stage1")
        {
            obj = (GameObject)Instantiate(loadSceneImage[2], BackGround.transform.position, Quaternion.identity);
            obj.transform.parent = BackGround.transform;
            loadSceneTextBox.sprite = loadSceneText[2];
            loadScene = "Level_Stage2";
        }
        else if (SceneData.Instance.referer == "Stage2")
        {
            obj = (GameObject)Instantiate(loadSceneImage[3], BackGround.transform.position, Quaternion.identity);
            obj.transform.parent = BackGround.transform;
            loadSceneTextBox.sprite = loadSceneText[3];
            loadScene = "Stage3";
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
        SceneManager.LoadScene(loadScene);
    }
}
