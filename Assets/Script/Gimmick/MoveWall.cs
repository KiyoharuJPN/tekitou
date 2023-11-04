using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    [System.Serializable]
    struct MoveWallStatus
    {
        [SerializeField, Header("プレイヤー")]
        public GameObject player;

        [SerializeField, Header("移動スピード 距離n以下の場合")]
        public float moveSpeed_below;
        [SerializeField, Header("移動スピード 距離n以上の場合")]
        public float moveSpeed_more;
        [SerializeField, Header("距離")]
        public float distance;

        [SerializeField, Header("到達地点X数値")]
        public float xNum;
    }

    [SerializeField, Header("動く壁設定")]
    MoveWallStatus moveWallStatus;

    private void Start()
    {
        MoveStart();
    }

    public void MoveStart()
    {
        StartCoroutine(MoveCheck());
    }

    IEnumerator MoveCheck()
    {
        Vector2 targetPos = new Vector2(moveWallStatus.xNum, transform.position.y);
        var dis = Vector2.Distance(transform.position, targetPos);
        while (dis > 0.1f)
        {
            if(moveWallStatus.player.GetComponent<PlayerController>().GetIsDead) { break; }
            Move(targetPos);
            dis = Vector2.Distance(transform.position, targetPos);
            yield return null;
        }

        MoveEnd();
    }

    void Move(Vector2 movePos)
    {
        this.transform.position = Vector2.MoveTowards(transform.position, movePos, MoveSpeedSet() * Time.deltaTime); // 目的の位置に移動
    }

    float MoveSpeedSet()
    {
        //指定されている距離よりPlayerとの距離がある場合
        if (Vector2.Distance(transform.position, moveWallStatus.player.transform.position) >= moveWallStatus.distance)
        {
            return moveWallStatus.moveSpeed_more;
        }
        else
        {
            Debug.Log(moveWallStatus.moveSpeed_below);
            return moveWallStatus.moveSpeed_below;
        }
    }

    void MoveEnd()
    {
    }
}
