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
    private GameObject cameraArea_Nomal;
    [SerializeField]
    private GameObject cameraArea_Boss;

    void Start()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Nomal.GetComponent<PolygonCollider2D>();
    }

    internal void ChengeCameraArea_Nomal()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Nomal.GetComponent<PolygonCollider2D>();
    }

    internal void ChengeCameraArea_Boss()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Boss.GetComponent<PolygonCollider2D>();
    }
}
