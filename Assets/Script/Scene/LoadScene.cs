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
    LoadFadeImage fade;
    string loadScene;

    //ロードシーン待機時間
    const float time = 2f;

    //多重発生防止bool
    bool canLoadScene = true;

    private void Start()
    {
        loadSceneTextBox.sprite = null;
        if (SceneData.Instance.referer == "Title")
        {
            loadSceneTextBox.sprite = loadSceneText[0];
            loadScene = "Level_Tutorial";
        }
        else if (SceneData.Instance.referer == "Tutorial")
        {
            loadSceneTextBox.sprite = loadSceneText[1];
            loadScene = "Level_Stage1";
        }
        else if (SceneData.Instance.referer == "Stage1")
        {
            loadSceneTextBox.sprite = loadSceneText[0];
            loadScene = "TitleScene";
        }
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
        SceneManager.LoadScene(loadScene);
    }
}
