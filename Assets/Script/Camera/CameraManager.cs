using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject CMvcam;


    [SerializeField]
    private GameObject cameraArea_Tutorial;
    [SerializeField]
    private GameObject cameraArea_Stage1Nomal;
    [SerializeField]
    private GameObject cameraArea_Stage1Boss;
    [SerializeField]
    private GameObject cameraArea_Stage2Nomal;
    [SerializeField]
    private GameObject cameraArea_Stage2Boss;

    void Start()
    {
        if(SceneData.Instance.referer == "Tutorial")
        {
            ChengeCameraArea_Tutorial();
        }
        else if(SceneData.Instance.referer == "Stage1")
        {
            ChengeCameraArea_Stage1Nomal();
        }
        else if (SceneData.Instance.referer == "Stage2")
        {
            ChengeCameraArea_Stage2Nomal();
        }
    }
    internal void ChengeCameraArea_Tutorial()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Tutorial.GetComponent<PolygonCollider2D>();
    }
    internal void ChengeCameraArea_Stage1Nomal()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage1Nomal.GetComponent<PolygonCollider2D>();
    }
    internal void ChengeCameraArea_Stage2Nomal ()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage2Nomal.GetComponent<PolygonCollider2D>();
    }

    internal void ChengeCameraArea_Boss()
    {
        if (SceneData.Instance.referer == "Stage1")
        {
            CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage1Boss.GetComponent<PolygonCollider2D>();
        }
        else if (SceneData.Instance.referer == "Stage2")
        {
            CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage2Boss.GetComponent<PolygonCollider2D>();
        }
    }
}
