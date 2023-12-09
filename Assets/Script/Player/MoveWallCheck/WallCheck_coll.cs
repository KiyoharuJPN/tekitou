using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck_coll : MonoBehaviour
{
    [SerializeField, Header("ï«Ç…êGÇÍÇƒÇ¢ÇÈÇ©")]
    public bool isWall = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stage" || collision.gameObject.tag == "MoveWall")
        {
            isWall = true;
            //Debug.Log("CheckWall" + transform.position + isWall);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stage" || collision.gameObject.tag == "MoveWall")
        {
            isWall = true;
            //Debug.Log("CheckWall" + transform.position + isWall);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stage" || collision.gameObject.tag == "MoveWall")
        {
            isWall = false;
            //Debug.Log("CheckWall" + transform.position + isWall);
        }
    }
}
