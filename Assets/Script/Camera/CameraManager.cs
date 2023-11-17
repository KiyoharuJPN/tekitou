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
    [SerializeField]
    private GameObject cameraArea_Stage3Nomal;
    [SerializeField]
    private GameObject cameraArea_Stage3Boss;

    void Start()
    {
        NomalCameraAreaSet();
    }
    internal void ChengeCameraArea_Tutorial()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Tutorial.GetComponent<PolygonCollider2D>();
        CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6.6f;
    }
    internal void ChengeCameraArea_Stage1Nomal()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage1Nomal.GetComponent<PolygonCollider2D>();
        CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6.6f;
    }
    internal void ChengeCameraArea_Stage2Nomal ()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage2Nomal.GetComponent<PolygonCollider2D>();
        CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6.6f;
    }
    internal void ChengeCameraArea_Stage3Nomal()
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage3Nomal.GetComponent<PolygonCollider2D>();
        CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6.6f;
    }
    internal void ChengeCameraArea_Boss()
    {
        if (SceneData.Instance.referer == "Stage1")
        {
            CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage1Boss.GetComponent<PolygonCollider2D>();
            CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5.3f;
        }
        else if (SceneData.Instance.referer == "Stage2")
        {
            CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage2Boss.GetComponent<PolygonCollider2D>();
            CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5.8f;
        }
        else if (SceneData.Instance.referer == "Stage3")
        {
            CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage3Boss.GetComponent<PolygonCollider2D>();
            CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 9.5f;
        }
        //TODO セイカフェス限定コード
        else if (SceneData.Instance.referer == "Seika_Stage1")
        {
            CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = cameraArea_Stage1Boss.GetComponent<PolygonCollider2D>();
            CMvcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5.3f;
        }
    }

    public void NomalCameraAreaSet()
    {
        if (SceneData.Instance.referer == "Tutorial")
        {
            ChengeCameraArea_Tutorial();
        }
        else if (SceneData.Instance.referer == "Stage1")
        {
            ChengeCameraArea_Stage1Nomal();
        }
        else if (SceneData.Instance.referer == "Stage2")
        {
            ChengeCameraArea_Stage2Nomal();
        }
        else if (SceneData.Instance.referer == "Stage3")
        {
            ChengeCameraArea_Stage3Nomal();
        }
        else if(SceneData.Instance.referer == "Demo")
        {
            ChengeCameraArea_Stage1Nomal();
        }

        //seikaフェス限定コード
        if (SceneData.Instance.referer == "Seika_Tutorial")
        {
            ChengeCameraArea_Tutorial();
        }
        else if (SceneData.Instance.referer == "Seika_Stage1")
        {
            ChengeCameraArea_Stage1Nomal();
        }
    }

    public void SetOriCameraArea(GameObject oriCameraArea)
    {
        CMvcam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = oriCameraArea.GetComponent<PolygonCollider2D>();
    }
}
