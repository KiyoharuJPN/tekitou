using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    private void Awake()
    {
        SceneData.Instance.referer = "Stage1";
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        ExAttackParam.Instance.SetGage(SceneData.Instance.ExGage);
        GameManager.Instance.PlayBGM(2);   
    }
}
