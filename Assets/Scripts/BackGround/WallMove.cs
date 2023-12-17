using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallMove : MonoBehaviour
{
    [SerializeField, Header("動かす背景")]
    GameObject[] walls;

    [SerializeField, Header("背景一番上")]
    GameObject wallTop;
    Vector3 topPos;
    [SerializeField, Header("背景一番下")]
    GameObject wallBottom;
    Vector3 bottomPos;

    [SerializeField, Header("スピード")]
    float moveSpeed;

    private void Start()
    {
        topPos = wallTop.transform.position;
        bottomPos = wallBottom.transform.position;
    }

    private void FixedUpdate()
    {
        //上方向に動かす
        foreach (GameObject wall in walls)
        {
            wall.transform.position += new Vector3 (Time.deltaTime * moveSpeed, 0);
        }

        //posチェック
        foreach (GameObject wall in walls)
        {
            if(wall.transform.position.x >= topPos.x)
            {
                wall.transform.position = bottomPos;
            }
        }
    }
}
