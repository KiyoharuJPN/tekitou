using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Seika_Load : MonoBehaviour
{
    [SerializeField]
    GameObject backGroundImage;

    [SerializeField]
    Image loadSceneTextBox;
    [SerializeField]
    Sprite[] loadSceneText;

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
        Cursor.visible = false;
        loadSceneTextBox.sprite = null;
        if (SceneData.Instance.referer == "Title")
        {
            obj = (GameObject)Instantiate(loadSceneImage[0], backGroundImage.transform.position, Quaternion.identity);
            obj.transform.parent = backGroundImage.transform;
            loadSceneTextBox.sprite = loadSceneText[0];
            loadScene = "Seika_Tutorial";
        }
        else if (SceneData.Instance.referer == "Seika_Tutorial")
        {
            obj = (GameObject)Instantiate(loadSceneImage[1], backGroundImage.transform.position, Quaternion.identity);
            obj.transform.parent = backGroundImage.transform;
            loadSceneTextBox.sprite = loadSceneText[1];
            loadScene = "Seika_Stage1";
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
