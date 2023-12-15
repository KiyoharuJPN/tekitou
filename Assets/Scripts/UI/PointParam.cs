using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointParam : MonoBehaviour
{
    public TextMeshProUGUI text;
    private int point, point_preb;

    public static PointParam Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        point = 0;
        //スコア初期化
        string SpriteText = point.ToString("d7");
        text.text = "";
        foreach (var i in SpriteText)
        {
            text.text += "<sprite=" + i + ">";
        }
        
        point_preb = 0;
    }

    //ゲットセット関数
    public int GetPoint()
    {
        return point;
    }

    public void SetPoint(int pt)
    {
        point = pt;

        string SpriteText = point.ToString("d7");
        text.text = "";
        foreach (var i in SpriteText)
        {
            text.text += "<sprite=" + i + ">";
        }
    }
}
