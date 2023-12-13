using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarpDoor_MoveWall : MonoBehaviour, IEventStart
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

    Collider2D m_Player;
    bool canDoor = true;

    InputAction move;
    private void Start()
    {
        warpPoint = transform.Find("WarpPoint").gameObject;
        isBottonUi = false;
    }

    public void EventStart(PlayerController player)
    {
        canDoor = false;
        Destroy(bottonUiPrefab);
        bottonUiPrefab = null;
        animator.SetTrigger("DoorOpen");
        SoundManager.Instance.PlaySE(SESoundData.SE.Door);
        player.WarpDoor(inPoint.transform);
        StartCoroutine(PlayerWarp(1.0f, player));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("InvinciblePlayer") && canDoor)
        {
            if (collision.GetComponent<PlayerController>().isGround)
            {
                m_Player = collision;
                isBottonUi = true;
                _BottonUi(collision);
            }
            else if (!collision.GetComponent<PlayerController>().isGround)
            {
                m_Player = null;
                Destroy(bottonUiPrefab);
                bottonUiPrefab = null;
                isBottonUi = false;
            }
        };
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        m_Player = null;
        Destroy(bottonUiPrefab);
        bottonUiPrefab = null;
        isBottonUi = false;
    }

    void _BottonUi(Collider2D player)
    {
        if (bottonUiPrefab != null) return;
        bottonUiPrefab =
        Instantiate(BottonUi, new Vector2(player.transform.position.x, player.transform.position.y + 2f), Quaternion.identity);

        bottonUiPrefab.transform.parent = player.transform;
    }

    IEnumerator PlayerWarp(float delay, PlayerController player)
    {

        GameManager.Instance.PlayTimeStop();
        //死んでいるEnemy強制削除
        DaedEnemyDestroy();
        player.SetCanMove(false);
        yield return new WaitForSeconds(delay);//渡された時間待機

        //フェードアウト開始
        fade.StartFadeOut();

        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        //フェードアウト終了
        ComboParam.Instance.ResetTime();
        m_Player.transform.position = warpPoint.transform.position;

        yield return new WaitForSeconds(1f);//渡された時間待機
        //フェードイン開始
        fade.StartFadeIn();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }

        player.WarpDoorEnd();

        player.SetCanMove(true);

        GameManager.Instance.PlayTimeStart();

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
            if (gameObj != null && gameObj.GetComponent<Enemy>())
            {
                if (gameObj.GetComponent<Enemy>().isDestroy)
                {
                    gameObj.GetComponent<Enemy>().EnemyNomalDestroy();
                }
            }
        }
    }
}
