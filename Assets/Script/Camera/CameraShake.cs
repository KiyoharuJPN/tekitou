using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    [Header("カメラオブジェクト")]
    GameObject CAMERA;

    /// <summary>
    /// 揺れ情報
    /// </summary>
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("揺れ時間")]
        public float Duration;
        [Tooltip("揺れの強さ")]
        public float Strength;
        [Tooltip("どのくらい振動するか")]
        public float Vibrato;
    }

    [SerializeField]
    [Header("画面揺れに関する")]
    public ShakeInfo _shakeInfo;

    private Vector2 _initPosition; // 初期位置
    public bool _isDoShake;       // 揺れ実行中か？
    private float _totalShakeTime; // 揺れ経過時間   

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
