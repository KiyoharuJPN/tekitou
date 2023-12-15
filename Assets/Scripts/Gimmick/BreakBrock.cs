using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBrock : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    public GameObject block;

    private void OnTriggerEnter2D(Collider2D col)
    {
        int layer = col.gameObject.layer;
        if (layer == 10)
        {
            Destroy(block);
            StartCoroutine(BreakBox());
        }
    }
    private IEnumerator BreakBox()
    {
        _particle.Play();
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
