using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    [Header("カメラオブジェクト")]
    GameObject CAMERA;
    public static CameraShake instance;

    int flag = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void _CameraShake(int flag)
    {
        this.flag = flag;
    }

    private void Update()
    {
        Debug.Log(flag);
        switch (flag) 
        {
            
            case 1:
                goto case 3;
            case 3:
                CAMERA.transform.Translate(0, 30 * Time.deltaTime, 0);        
                if (CAMERA.transform.position.y >= 1.0f)
                    flag++;
                break;
            case 2:
                CAMERA.transform.Translate(0, -30 * Time.deltaTime, 0);
                if (CAMERA.transform.position.y <= -1.0f)
                    flag++;
                break;
            case 4:
                CAMERA.transform.Translate(-30 * Time.deltaTime, 0, 0);
                if (CAMERA.transform.position.x <= 0)
                    flag = 0;
                break;
        }
    }
}
