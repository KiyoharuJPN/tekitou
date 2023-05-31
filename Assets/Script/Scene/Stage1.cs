using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneData.Instance.referer = "Stage1";
        GameManager.Instance.PlayBGM(2);   
    }
}
