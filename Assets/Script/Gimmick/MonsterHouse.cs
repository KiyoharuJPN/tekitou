using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHouse : MonoBehaviour
{

    [SerializeField, Header("封鎖壁タイルマップ")]
    GameObject wallTileMap;

    [System.Serializable]
    struct EnemyWave
    {
        [System.Serializable]
        public struct PopEnemy
        {
            public GameObject enmey;
            public GameObject popPostion;
        }

        [SerializeField,Header("出現する敵・位置")]
        public PopEnemy[] popEnemys;
    }

    [SerializeField]
    EnemyWave[] enemyWaves;

    //召喚された敵を格納
    public List<GameObject> enemylist = new List<GameObject>();

    //プレイヤーが入室したかどうか
    private bool roomInPlayer = false;

    //召喚アニメーション
    [SerializeField, Header("召喚アニメーション")]
    GameObject summonAnimObj;

    //現在のWave
    private int waveNum = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //連続発生防止
        if (!roomInPlayer) 
        {
            roomInPlayer = true;
            this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            wallTileMap.SetActive(true);
            //敵を召喚
            StartCoroutine(NextWave());
        }
    }

    /// <summary>
    /// 次のWaveセット
    /// </summary>
    IEnumerator NextWave()
    {
        //必殺技対策
        bool check = false;
        while (GameManager.Instance.isPlayerExSkill)
        {
            check = true;
            yield return null;
        }
        if(check) yield return new WaitForSeconds(1);

        enemylist.Clear();
        //敵生成
        foreach (var enemy in enemyWaves[waveNum].popEnemys)
        {
            Instantiate(summonAnimObj, enemy.popPostion.transform.position, Quaternion.identity);
            var enemyObj = Instantiate(enemy.enmey, enemy.popPostion.transform.position, Quaternion.identity);
            enemyObj.AddComponent<MonsterHouse_Enemy>();
            enemyObj.GetComponent<MonsterHouse_Enemy>().monsterHouse = this;
            enemylist.Add(enemyObj);
        }

        StartCoroutine(ClearCheck());
        waveNum++;
    }

    /// <summary>
    /// そのWaveをクリアしたか
    /// </summary>
    IEnumerator ClearCheck()
    {

        //敵が全て死んでいたら
        if (enemylist.Count == 0)
        {
            //waveが終わりであれば
            if(enemyWaves.Length == waveNum)
            {
                yield return new WaitForSeconds(1);
                MonsterHouseEnd();
            }
            else //waveが終わりでない場合
            {   
                yield return new WaitForSeconds(1);  
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                StartCoroutine(NextWave());//次のWaveを発生
            }
        }
        else //死んでいなければ
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(ClearCheck());
        }
    }

    public void EnemyListRemove(GameObject enemy)
    {
        enemylist.Remove(enemy);
    }

    void MonsterHouseEnd()
    {
        Destroy(this.gameObject);
    }
}
