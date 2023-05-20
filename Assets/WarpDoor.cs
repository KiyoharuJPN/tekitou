using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] internal Animator animator;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("“ü—Í‰Â”\");
        float lsv = Input.GetAxis("L_Stick_V");
        if (lsv <= -0.8)
        {
            animator.SetTrigger("DoorOpen");
        }
    }
}
