using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHouse : MonoBehaviour
{

    [SerializeField, Header("�����ǃ^�C���}�b�v")]
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

        [SerializeField,Header("�o������G�E�ʒu")]
        public PopEnemy[] popEnemys;
    }

    [SerializeField]
    EnemyWave[] enemyWaves;

    //�������ꂽ�G���i�[
    public List<GameObject> enemylist = new List<GameObject>();

    //�v���C���[�������������ǂ���
    private bool roomInPlayer = false;

    //�����A�j���[�V����
    [SerializeField, Header("�����A�j���[�V����")]
    GameObject summonAnimObj;

    //���݂�Wave
    private int waveNum = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�A�������h�~
        if (!roomInPlayer) 
        {
            roomInPlayer = true;
            this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            wallTileMap.SetActive(true);
            //�G������
            NextWave();
        }
    }

    /// <summary>
    /// ����Wave�Z�b�g
    /// </summary>
    void NextWave()
    {
        enemylist.Clear();
        //�G����
        foreach (var enemy in enemyWaves[waveNum].popEnemys)
        {
            enemylist.Add(Instantiate(enemy.enmey, enemy.popPostion.transform.position, Quaternion.identity));
        }

        StartCoroutine(ClearCheck());
        waveNum++;
    }

    /// <summary>
    /// ����Wave���N���A������
    /// </summary>
    IEnumerator ClearCheck()
    {
        List<GameObject> objlist = new List<GameObject>();
        foreach (var enemy in enemylist)
        {
            //�G�����S��Ԃ�
            if (enemy.GetComponent<Enemy>().isDestroy)
            {
                objlist.Add(enemy);
            }
        }
        foreach (var item in objlist)
        {
            enemylist.Remove(item);
        }

        //�G���S�Ď���ł�����
        if (enemylist.Count == 0)
        {
            //wave���I���ł����
            if(enemyWaves.Length == waveNum)
            {
                yield return new WaitForSeconds(1);
                MonsterHouseEnd();
            }
            else //wave���I���łȂ��ꍇ
            {
                yield return new WaitForSeconds(1);
                NextWave();//����Wave�𔭐�
            }
        }
        else //����ł��Ȃ����
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(ClearCheck());
        }
    }

    void MonsterHouseEnd()
    {
        Destroy(this.gameObject);
    }
}
