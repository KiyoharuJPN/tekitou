using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyStage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name + "íœ");
        Destroy(other.gameObject);
    }
}
