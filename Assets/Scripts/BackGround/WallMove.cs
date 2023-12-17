using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallMove : MonoBehaviour
{
    [SerializeField, Header("“®‚©‚·”wŒi")]
    GameObject[] walls;

    [SerializeField, Header("”wŒiˆê”Ôã")]
    GameObject wallTop;
    Vector3 topPos;
    [SerializeField, Header("”wŒiˆê”Ô‰º")]
    GameObject wallBottom;
    Vector3 bottomPos;

    [SerializeField, Header("ƒXƒs[ƒh")]
    float moveSpeed;

    private void Start()
    {
        topPos = wallTop.transform.position;
        bottomPos = wallBottom.transform.position;
    }

    private void FixedUpdate()
    {
        //ã•ûŒü‚É“®‚©‚·
        foreach (GameObject wall in walls)
        {
            wall.transform.position += new Vector3 (Time.deltaTime * moveSpeed, 0);
        }

        //posƒ`ƒFƒbƒN
        foreach (GameObject wall in walls)
        {
            if(wall.transform.position.x >= topPos.x)
            {
                wall.transform.position = bottomPos;
            }
        }
    }
}
