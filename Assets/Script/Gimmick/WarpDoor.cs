using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WarpDoor : MonoBehaviour
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
    [SerializeField, Header("ボス部屋前かどうか")]
    bool bossDoor;
    GameObject warpPoint;

    Collider2D player;
    bool canDoor = true;

    InputAction  move;
    private void Start()
    {
        warpPoint = transform.Find("WarpPoint").gameObject;
        isBottonUi = false;

        var playerInput = GetComponent<PlayerInput>();
        move = playerInput.actions["Move"];
    }

    private void Update()
    {
        if (player == null) return;

        float lsv = move.ReadValue<Vector2>().y;
        if (lsv >= 0.8 && canDoor)
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("InvinciblePlayer"))
        {
            if(collision.GetComponent<PlayerController>().isGround)
            {
                player = collision;
                isBottonUi = true;
                _BottonUi(collision);
            }
            else if (!collision.GetComponent<PlayerController>().isGround)
            {
                player = null;
                Destroy(bottonUiPrefab);
                bottonUiPrefab = null;
                isBottonUi = false;
            }
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
        if (bottonUiPrefab != null) return;
        bottonUiPrefab =
        Instantiate(BottonUi, new Vector2(player.transform.position.x, player.transform.position.y + 2f), Quaternion.identity);

        bottonUiPrefab.transform.parent = player.transform;
    }

    IEnumerator PlayerWarp(float delay,Collider2D player)
    {
        GameManager.Instance.PlayTimeStop();
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
        if (SceneData.Instance.referer != "Tutorial" && bossDoor) camera.ChengeCameraArea_Boss();

        yield return new WaitForSeconds(1f);//渡された時間待機
        //フェードイン開始
        fade.StartFadeIn();
        while (!fade.IsFadeOutComplete())
        {
            yield return null;
        }
        if(bossDoor)
        {
            GameManager.Instance.isBossRoom = true;

            if (SceneData.Instance.referer == "Stage3")
            {
                GameManager.Instance.BGMStart_BossRoom2();
            }
            else
            {
                GameManager.Instance.BGMStart_BossRoom();
            }
        }

        player.GetComponent<PlayerController>().WarpDoorEnd();

        yield return new WaitForSeconds(1f);
        if (!bossDoor)
        {
            player.GetComponent<PlayerController>().SetCanMove(true);
        }


        GameManager.Instance.PlayTimeStart();
    }

    //死んでいるEnemy強制削除
    void DaedEnemyDestroy()
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject gameObj in enemys)
        {
            if(gameObj.GetComponent<Enemy>() != null)
            {
                if (gameObj.GetComponent<Enemy>() == null) return;
                if (gameObj.GetComponent<Enemy>().isDestroy)
                {
                    gameObj.GetComponent<Enemy>().EnemyNomalDestroy();
                }
            }
        }
    }
}
