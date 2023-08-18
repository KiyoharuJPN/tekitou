using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCtrl : MonoBehaviour
{
    [Header("�v���C���[�Q�[���I�u�W�F�N�g")] public GameObject playerObj;
    [Header("�R���e�B�j���[�ʒu")] public GameObject[] continuePoint;

    virtual protected void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0 && !SceneData.Instance.wayPoint)
        {
            playerObj.transform.position = continuePoint[0].transform.position;
        }
        else if(playerObj != null && continuePoint != null && continuePoint.Length > 0 && SceneData.Instance.wayPoint)
        {
            playerObj.transform.position = continuePoint[1].transform.position;
        }
        else
        {
            Debug.Log("�ݒ肪����ĂȂ���I");
        }
    }
}