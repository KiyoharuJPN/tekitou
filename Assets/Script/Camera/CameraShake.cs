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

    public void Shake(float duration, float magnitude, bool vertical, bool horizontal)
    {
        StartCoroutine(DoShake(duration, magnitude, vertical, horizontal));
    }
    private IEnumerator DoShake(float duration, float magnitude, bool vertical, bool horizontal)
    {
        this.gameObject.GetComponent<CinemachineBrain>().enabled = false;
        var pos = transform.localPosition;

        var elapsed = 0f;
        _isDoShake = true;

        while (elapsed < duration)
        {
            var x = pos.x;
            var y = pos.y;

            if (vertical)
            {
                x = pos.x + Random.Range(-1f, 1f) * magnitude;
            }
            if (horizontal)
            {
                y = pos.y + Random.Range(-1f, -1f) * magnitude;
            }

            transform.localPosition = new Vector3(x, y, pos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        _isDoShake = false;
        transform.localPosition = pos;
        this.gameObject.GetComponent<CinemachineBrain>().enabled = true;
    }
}
