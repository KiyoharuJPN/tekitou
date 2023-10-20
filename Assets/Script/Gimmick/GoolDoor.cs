using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoolDoor : MonoBehaviour
{
    [SerializeField] FadeImage fade;
    [SerializeField] new CameraManager camera;

    Collider2D player;

    [SerializeField]
    GameObject BottonUi;
    [SerializeField]
    GameObject inPoint;
    GameObject bottonUiPrefab;
    bool isBottonUi;

    bool canDoor = true;

    private void Start()
    {
        isBottonUi = false;
    }

    private void Update()
    {
        if (player == null) return;

        float lsv = Input.GetAxis("L_Stick_V");
        if ((lsv >= 0.8 || Input.GetKeyDown(KeyCode.K)) && canDoor)
        {
            canDoor = false;
            Destroy(bottonUiPrefab);
            bottonUiPrefab = null;
            GameManager.Instance.EnemyStop_Start();
            player.GetComponent<PlayerController>().GoolDoor(inPoint.transform);
            StartCoroutine(Result(2.0f, player));
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 && !isBottonUi)
        {
            player = collision;
            isBottonUi = true;
            _BottonUi(collision);
        };

    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        Destroy(bottonUiPrefab);
        bottonUiPrefab = null;
        isBottonUi = false;
        player = null;
    }

    void _BottonUi(Collider2D player)
    {
        bottonUiPrefab =
        Instantiate(BottonUi, new Vector2(player.transform.position.x, player.transform.position.y + 2f), Quaternion.identity);

        bottonUiPrefab.transform.parent = player.transform;
    }

    IEnumerator Result(float delay,Collider2D player)
    {
        player.GetComponent<PlayerController>().SetCanMove(false);
        yield return new WaitForSeconds(delay);//渡された時間待機

        //フェードアウト開始
        fade.StartFadeOut();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
        //フェードアウト終了

        GameManager.Instance.EnemyStop_Start();
        GameManager.Instance.Result_Start(0);
    }
}
