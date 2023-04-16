using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointParam : MonoBehaviour
{
    public Text text;
    private int point, point_preb;
    // Start is called before the first frame update
    void Start()
    {
        text.text = 0.ToString("d15");
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
            text.text = (point).ToString("d15");
            point_preb = point;
        }
    }

    public int GetPoint()
    {
        return point;
    }

    public void SetPoint(int pt)
    {
        point = pt;
    }
}
