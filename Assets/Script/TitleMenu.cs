using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("接続したいシーンの名前を入れてください"),Header("接続したいシーンの名前")]
    public string gameScene;
    [Tooltip("タイトル画像を入れてください"),Header("タイトル画像")]
    public GameObject TitleImage;

    public GameObject[] menuobj;

    public GameObject[] optionShow;

    public GameObject[] SEoptionShow;
    [Tooltip("SEオプションコントローラを入れてください"), Header("SEオプション画面")]
    public GameObject SEdisplay;
    public Slider masterVolume, BGMVolume, SEVolume;

    int pointer;
    int pointerpreb;

    bool volumeChecking = false, hideKeyChecking = false;

    //Sound
    public float master = 0.4f, BGM = 0.4f, SE = 0.4f;

    private void Start()
    {
        pointer = 0;
        OnSelected(menuobj[0]);

        //音声修正
        SoundManager.Instance.masterVolume = master;
        SoundManager.Instance.bgmMasterVolume = BGM;
        SoundManager.Instance.seMasterVolume = SE;
        masterVolume.value = master;
        BGMVolume.value = BGM;
        SEVolume.value = SE;
    }

    private void Update()
    {
        //調整キーの設定
        if (Input.GetKeyDown(KeyCode.W))
        {
            pointer--;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            pointer++;
        }

        if(pointer != pointerpreb)//変更されたときの作業
        {
            if (menuobj[0].activeSelf)//Menu
            {
                if (pointer < 0) pointer = menuobj.Length - 1;
                if (pointer > menuobj.Length - 1) pointer = 0;//上限調整

                OnSelected(menuobj[pointer]);
                if(pointer !=pointerpreb && pointerpreb != -1) OnDeselected(menuobj[pointerpreb]);
            }

            if (optionShow[0].activeSelf)//Option
            {
                if (pointer < 0) pointer = optionShow.Length - 1;
                if (pointer > optionShow.Length - 1) pointer = 0;//上限調整

                OnSelected(optionShow[pointer]);
                Debug.Log("p" + pointer + '\n' + "pp" + pointerpreb);
                if (pointer != pointerpreb && pointerpreb != -1) OnDeselected(optionShow[pointerpreb]);
            }

            if (SEoptionShow[0].activeSelf)//Sound
            {
                if (pointer < 0) pointer = SEoptionShow.Length - 1;
                if (pointer > SEoptionShow.Length - 1) pointer = 0;//上限調整

                OnSelected(SEoptionShow[pointer]);
                if (pointer != pointerpreb && pointerpreb != -1)
                {
                    if (pointerpreb > 0)
                    {
                        OnDeselectedSE(SEoptionShow[pointerpreb]);
                    }
                    else
                    {
                        OnDeselected(SEoptionShow[pointerpreb]);
                    }
                }
            }


            //ポインターの修正
            pointerpreb = pointer;
        }




        //選択キーの設定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (menuobj[0].activeSelf && !hideKeyChecking)//Menu
            {
                switch (pointer)
                {
                    case 0:
                        GameStart();
                        OnDeselected(menuobj[pointer]);
                        break;
                    case 1:
                        Option();
                        OnDeselected(menuobj[pointer]);
                        break;
                    case 2:
                        Exit();
                        OnDeselected(menuobj[pointer]);
                        break;
                    default:
                        if (pointer < menuobj.Length - 1)
                        {
                            OnDeselected(menuobj[pointer]);
                            Debug.Log("新しい項目を追加するときはプログラマに頼んでください。");
                        }
                        break;
                }
                hideKeyChecking = true;
            }

            if (optionShow[0].activeSelf && !hideKeyChecking)//Option
            {
                switch (pointer)
                {
                    case 0:
                        //保存しておく
                        masterVolume.value = master = SoundManager.Instance.masterVolume;
                        BGMVolume.value = BGM = SoundManager.Instance.bgmMasterVolume;
                        SEVolume.value = SE = SoundManager.Instance.seMasterVolume;
                        //画面を開く
                        SEOption();
                        OnDeselected(optionShow[pointer]);
                        volumeChecking = true;
                        break;
                    default:
                        if(pointer < optionShow.Length - 1)
                        {
                            OnDeselected(optionShow[pointer]);
                            Debug.Log("新しい項目を追加するときはプログラマに頼んでください。");
                        }
                        break;
                }
                hideKeyChecking = true;
            }

            if (SEoptionShow[0].activeSelf && !hideKeyChecking)
            {
                switch (pointer)
                {
                    case 0:
                        volumeChecking = false;
                        OnDeselected(SEoptionShow[pointer]);
                        DeSEOption();
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    default:
                        if (pointer < SEoptionShow.Length - 1) OnDeselectedSE(SEoptionShow[pointer]);
                        Debug.Log("新しい項目を追加するときはプログラマに頼んでください。");
                        break;
                }
                hideKeyChecking = true;
            }

            //ポインターの復元
            if (!volumeChecking)
            {
                pointer = 0;
                pointerpreb = -1;
            }
            hideKeyChecking = false;
        }
        if (volumeChecking) SetVolume();





        //戻るキーの設定
        if (Input.GetKeyDown(KeyCode.B))
        {
            //menu
            if (menuobj[0].activeSelf)
            {
                pointer = 0;
            }

            //option
            if (optionShow[0].activeSelf)
            {
                OnDeselected(optionShow[pointer]);
                DeOption();
            }

            //sound
            if (SEoptionShow[0].activeSelf)
            {
                volumeChecking = false;

                if (pointer > 0)
                {
                    OnDeselectedSE(SEoptionShow[pointer]);
                }
                else
                {
                    OnDeselected(SEoptionShow[pointer]);
                }
                //ボリュームを戻す
                SoundManager.Instance.masterVolume = master;
                SoundManager.Instance.bgmMasterVolume = BGM;
                SoundManager.Instance.seMasterVolume = SE;

                DeSEOption();
            }

            //ポインターの復元
            pointer = 0;
            pointerpreb = -1;
        }
    }

    //メニューの動き　　これを無視して?//.getcompeont<Code>().getmove();
    void GameStart()
    {
        if(gameScene!="")SceneManager.LoadScene(gameScene);
        Debug.Log("ゲーム画面をロードしました。");
    }

    void Option()
    {
        for(int i = 0; i < menuobj.Length; i++)
        {
            menuobj[i].SetActive(false);
        }
        for(int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(true);
        }
    }
    void DeOption()
    {
        for (int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(false);
        }
        for (int i = 0; i < menuobj.Length; i++)
        {
            menuobj[i].SetActive(true);
        }
    }
    void Exit()
    {
        Application.Quit();
        Debug.Log("ゲームから抜けました。");
    }

    void SEOption()
    {
        TitleImage.SetActive(false);
        for(int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(false);
        }
        SEdisplay.SetActive(true);
        for ( int i = 0; i < SEoptionShow.Length; i++)
        {
            SEoptionShow[i].SetActive(true);
        }
    }
    void DeSEOption()
    {
        for (int i = 0; i < SEoptionShow.Length; i++)
        {
            SEoptionShow[i].SetActive(false);
        }
        SEdisplay.SetActive(false);

        TitleImage.SetActive(true);
        for (int i = 0; i < optionShow.Length; i++)
        {
            optionShow[i].SetActive(true);
        }
    }





    //音声調整
    void SetVolume()
    {
        
        switch (pointer)
        {
            case 0:
                break;
            case 1:
                if (Input.GetKey(KeyCode.A))
                {
                    SoundManager.Instance.masterVolume = SoundManager.Instance.masterVolume + -0.001f;
                }else if (Input.GetKey(KeyCode.D))
                {
                    SoundManager.Instance.masterVolume = SoundManager.Instance.masterVolume + 0.001f;
                }
                masterVolume.value = SoundManager.Instance.masterVolume;
                break;
            case 2:
                if (Input.GetKey(KeyCode.A))
                {
                    SoundManager.Instance.bgmMasterVolume = SoundManager.Instance.bgmMasterVolume + -0.001f;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    SoundManager.Instance.bgmMasterVolume = SoundManager.Instance.bgmMasterVolume + 0.001f;
                }
                BGMVolume.value = SoundManager.Instance.bgmMasterVolume;
                break;
            case 3:
                if (Input.GetKey(KeyCode.A))
                {
                    SoundManager.Instance.seMasterVolume = SoundManager.Instance.seMasterVolume + -0.001f;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    SoundManager.Instance.seMasterVolume = SoundManager.Instance.seMasterVolume + 0.001f;
                }
                SEVolume.value = SoundManager.Instance.seMasterVolume;
                break;
            default:
                break;
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
    void OnDeselectedSE(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255, 0); //色を戻す
    }
}
