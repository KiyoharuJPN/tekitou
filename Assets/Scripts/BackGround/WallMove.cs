using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallMove : MonoBehaviour
{
    [SerializeField, Header("�������w�i")]
    GameObject[] walls;

    [SerializeField, Header("�w�i��ԏ�")]
    GameObject wallTop;
    Vector3 topPos;
    [SerializeField, Header("�w�i��ԉ�")]
    GameObject wallBottom;
    Vector3 bottomPos;

    [SerializeField, Header("�X�s�[�h")]
    float moveSpeed;

    private void Start()
    {
        topPos = wallTop.transform.position;
        bottomPos = wallBottom.transform.position;
    }

    private void FixedUpdate()
    {
        //������ɓ�����
        foreach (GameObject wall in walls)
        {
            wall.transform.position += new Vector3 (Time.deltaTime * moveSpeed, 0);
        }

        //pos�`�F�b�N
        foreach (GameObject wall in walls)
        {
            if(wall.transform.position.x >= topPos.x)
            {
                wall.transform.position = bottomPos;
            }
        }
    }
}
