using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    [Header("�J�����I�u�W�F�N�g")]
    GameObject CAMERA;

    /// <summary>
    /// �h����
    /// </summary>
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("�h�ꎞ��")]
        public float Duration;
        [Tooltip("�h��̋���")]
        public float Strength;
        [Tooltip("�ǂ̂��炢�U�����邩")]
        public float Vibrato;
    }

    [SerializeField]
    [Header("��ʗh��Ɋւ���")]
    public ShakeInfo _shakeInfo;

    private Vector2 _initPosition; // �����ʒu
    public bool _isDoShake;       // �h����s�����H
    private float _totalShakeTime; // �h��o�ߎ���   

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
        if (flag == 2) 
        {
            _isDoShake = true;
            CAMERA.transform.Translate(0, _shakeInfo.Strength * Time.deltaTime, 0);
            CAMERA.transform.Translate(0, -_shakeInfo.Strength * Time.deltaTime, 0);
            _totalShakeTime += Time.deltaTime;
            if (_shakeInfo.Vibrato < _totalShakeTime)
            {

                flag = 0;
                _totalShakeTime = 0;
                _isDoShake = false;
            }
        }
    }
}
