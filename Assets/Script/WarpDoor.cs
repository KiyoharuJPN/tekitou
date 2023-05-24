using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] internal Animator animator;
    [SerializeField] FadeImage fade;
    [SerializeField] CameraManager camera;

    [SerializeField]
    GameObject BottonUi;
    GameObject bottonUiPrefab;
    bool isBottonUi;

    GameObject warpPoint;

    private void Start()
    {
        warpPoint = transform.Find("WarpPoint").gameObject;
        isBottonUi = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !isBottonUi)
        {
            isBottonUi = true;
            _BottonUi(collision);
        };

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //衝突している物のレイヤーがPlayer(6番レイヤー）でなければ returnする
        if (collision.gameObject.layer != 6) return;

        float lsv = Input.GetAxis("L_Stick_V");
        if (lsv >= 0.8)
        {
            Destroy(bottonUiPrefab);
            bottonUiPrefab = null;
            animator.SetTrigger("DoorOpen");
            collision.GetComponent<PlayerController>().WarpDoor();
            StartCoroutine(PlayerWarp(3.0f, collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(bottonUiPrefab);
        bottonUiPrefab = null;
        isBottonUi = false;
    }

    void _BottonUi(Collider2D player)
    {
        Debug.Log(player.name);
        bottonUiPrefab =
        Instantiate(BottonUi, new Vector2(player.transform.position.x, player.transform.position.y + 2f), Quaternion.identity);

        bottonUiPrefab.transform.parent = player.transform;
    }

    IEnumerator PlayerWarp(float delay,Collider2D player)
    {
        yield return new WaitForSeconds(delay);//渡された時間待機

        //フェードアウト開始
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
        //フェードアウト終了

        player.transform.position = warpPoint.transform.position;
        camera.ChengeCameraArea_Boss();
        yield return new WaitForSeconds(1f);//渡された時間待機

        //フェードイン開始
        fade.StartFadeIn();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
        player.GetComponent<PlayerController>().WarpDoorEnd();
    }
}
