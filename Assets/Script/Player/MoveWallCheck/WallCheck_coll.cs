using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck_coll : MonoBehaviour
{
    [SerializeField, Header("�ǂɐG��Ă��邩")]
    public bool isWall = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            isWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            isWall = false;
        }
    }
}
