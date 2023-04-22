using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointParam : MonoBehaviour
{
    public Text text;
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
        text.text = "<size=20>SCORE   </size>" + (point).ToString("d8");        //スコアボードの初期化
        point = 0;
        point_preb = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (point < 0) point = 0;
        //Debug.Log(point);
        if (point != point_preb)
        {
            text.text = "<size=20>SCORE   </size>" + (point).ToString("d8");   //リアルタイム得点更新
            point_preb = point;
        }
    }

    //ゲットセット関数
    public int GetPoint()
    {
        return point;
    }

    public void SetPoint(int pt)
    {
        point = pt;
    }
}
