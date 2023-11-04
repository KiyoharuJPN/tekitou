using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpDoor_MoveWall : MonoBehaviour
{
    [SerializeField] internal Animator animator;
    [SerializeField] FadeImage fade;
    [SerializeField] private new CameraManager camera;

    [SerializeField]
    GameObject BottonUi;
    [SerializeField]
    GameObject inPoint;
    GameObject bottonUiPrefab;
    bool isBottonUi;
    GameObject warpPoint;

    [SerializeField]
    GameObject moveWall;

    Collider2D player;
    bool canDoor = true;
    private void Start()
    {
        warpPoint = transform.Find("WarpPoint").gameObject;
        isBottonUi = false;
    }

    private void Update()
    {
        if (player == null) return;

        float lsv = Input.GetAxis("L_Stick_V");
        if ((lsv >= 0.8 || Input.GetKeyDown(KeyCode.H)) && canDoor)
        {
            canDoor = false;
            Destroy(bottonUiPrefab);
            bottonUiPrefab = null;
            animator.SetTrigger("DoorOpen");
            SoundManager.Instance.PlaySE(SESoundData.SE.Door);
            player.GetComponent<PlayerController>().WarpDoor(inPoint.transform);
            StartCoroutine(PlayerWarp(1.0f, player));
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("InvinciblePlayer"))
        {
            player = collision;
            isBottonUi = true;
            _BottonUi(collision);
        };
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        player = null;
        Destroy(bottonUiPrefab);
        bottonUiPrefab = null;
        isBottonUi = false;
    }

    void _BottonUi(Collider2D player)
    {
        bottonUiPrefab =
        Instantiate(BottonUi, new Vector2(player.transform.position.x, player.transform.position.y + 2f), Quaternion.identity);

        bottonUiPrefab.transform.parent = player.transform;
    }

    IEnumerator PlayerWarp(float delay, Collider2D player)
    {
        //死んでいるEnemy強制削除
        DaedEnemyDestroy();
        player.GetComponent<PlayerController>().SetCanMove(false);
        yield return new WaitForSeconds(delay);//渡された時間待機

        //フェードアウト開始
        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        //フェードアウト終了
        ComboParam.Instance.ResetTime();
        player.transform.position = warpPoint.transform.position;

        yield return new WaitForSeconds(1f);//渡された時間待機
        //フェードイン開始
        fade.StartFadeIn();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        player.GetComponent<PlayerController>().WarpDoorEnd();

        player.GetComponent<PlayerController>().SetCanMove(true);

        yield return new WaitForSeconds(0.2f);
        //動く壁起動
        SoundManager.Instance.PlaySE(SESoundData.SE.moveWall);
        moveWall.SetActive(true);

    }

    //死んでいるEnemy強制削除
    void DaedEnemyDestroy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject gameObj in enemys)
        {
            if (gameObj.GetComponent<Enemy>().isDestroy)
            {
                gameObj.GetComponent<Enemy>().EnemyNomalDestroy();
            }
        }
    }
}
