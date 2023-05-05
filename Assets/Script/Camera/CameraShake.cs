using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    [Header("ƒJƒƒ‰ƒIƒuƒWƒFƒNƒg")]
    GameObject CAMERA;

    /// <summary>
    /// —h‚êî•ñ
    /// </summary>
    [System.Serializable]
    public struct ShakeInfo
    {
        [Tooltip("—h‚êŠÔ")]
        public float Duration;
        [Tooltip("—h‚ê‚Ì‹­‚³")]
        public float Strength;
        [Tooltip("‚Ç‚Ì‚­‚ç‚¢U“®‚·‚é‚©")]
        public float Vibrato;
    }

    [SerializeField]
    [Header("‰æ–Ê—h‚ê‚ÉŠÖ‚·‚é")]
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
