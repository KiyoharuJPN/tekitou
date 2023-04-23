using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FinishMenu : MonoBehaviour
{
    [Tooltip("目標のシーンネームを書いてください")]
    public string SceneName;


    public GameObject[] Finishobj;

    //ポインター
    int pointer = 0, pointerpreb = -1;

    private void Awake()
    {
        OnSelected(Finishobj[0]);
    }
    // Update is called once per frame
    void Update()
    {
        //調整キーの設定
        if(Input.GetKeyDown(KeyCode.W))
        {
            pointer--;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            pointer++;
        }


        //ポインターが変わった時の設定
        if(pointer != pointerpreb)
        {
            if (pointer < 0) pointer = Finishobj.Length - 1;
            if (pointer > Finishobj.Length - 1) pointer = 0;

            OnSelected(Finishobj[pointer]);
            Debug.Log("pointer"+pointer + '\n' + "pointerpreb"+pointerpreb);
            if (pointer != pointerpreb && pointerpreb != -1) OnDeselected(Finishobj[pointerpreb]);

            //ポインターを代入する
            pointerpreb = pointer;
        }

        //選択キーの指定
        if (Input.GetKeyUp(KeyCode.Space))
        {
            switch (pointer)
            {
                case 0:
                    Debug.Log(SystemController.Instance.GetLastScene());
                    SceneManager.LoadScene(SystemController.Instance.GetLastScene());
                    break;
                case 1:
                    if (SceneName != "") SceneManager.LoadScene(SceneName);
                    break;
                default:
                    Debug.Log("新しい項目の追加はプログラマに頼んでください。");
                    break;
            }
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
