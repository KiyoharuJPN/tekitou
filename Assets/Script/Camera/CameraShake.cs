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

    internal bool _isDoShake = false;

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        var pos = transform.localPosition;

        var elapsed = 0f;
        _isDoShake = true;

        while (elapsed < duration)
        {
            var y = pos.y + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(pos.x, y, pos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        _isDoShake = false;
        transform.localPosition = pos;
    }
}
