using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyStage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name + "�폜");
        Destroy(other.gameObject);
    }
}
