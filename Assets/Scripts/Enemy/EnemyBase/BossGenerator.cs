using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGenerator : MonoBehaviour
{
    [Tooltip("入れた敵オブジェクトはここに召喚される。")]
    public GameObject Boss;
    [Tooltip("何秒後召喚されるかを決める")]
    public float waitSecond;

    //召喚関連
    bool summon = true;
    //召喚位置
    Vector3 BossPos;
    private void Start()
    {
        BossPos = new Vector3(transform.position.x, transform.position.y + 10,transform.position.z);
    }

    private void OnBecameVisible()
    {
        if (summon)
        {
            StartCoroutine(BossSummon());
            summon = false;
            GameObject.Find("Hero").GetComponent<PlayerController>().SetCanMove(false);
        }
    }

    IEnumerator BossSummon()
    {
        yield return new WaitForSeconds(waitSecond);
        Instantiate(Boss,BossPos,Quaternion.identity);
        Destroy(gameObject);
    }
}
