using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] internal Animator animator;

    GameObject warpPoint;

    private void Start()
    {
        warpPoint = transform.Find("WarpPoint").gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //衝突している物のレイヤーがPlayer(6番レイヤー）でなければ returnする
        if (collision.gameObject.layer != 6) return;

        float lsv = Input.GetAxis("L_Stick_V");
        if (lsv >= 0.8)
        {
            animator.SetTrigger("DoorOpen");

            //TODO　現在ではフェードインを未実装の為コルーチンで実装、修正予定
            StartCoroutine(PlayerWarp(3.0f, collision));
        }
    }

    IEnumerator PlayerWarp(float delay,Collider2D collider)
    {
        yield return new WaitForSeconds(delay);
        collider.transform.position = warpPoint.transform.position;
    }
}
