using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    [Header("カメラオブジェクト")]
    GameObject CAMERA;



    internal bool _isDoShake = false;

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        this.gameObject.GetComponent<CinemachineBrain>().enabled = false;
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
        this.gameObject.GetComponent<CinemachineBrain>().enabled = true;
    }
}
