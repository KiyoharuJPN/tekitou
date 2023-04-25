using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBrock : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;
    private SpriteRenderer _rend;
    private void Awake()
    {
        _rend = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        int layer = col.gameObject.layer;
        if (layer == 10)
        {
            StartCoroutine(BreakBox());
        }
    }
    private IEnumerator BreakBox()
    {
        _rend.enabled = false;
        _particle.Play();
        yield return new WaitForSeconds(_particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}
