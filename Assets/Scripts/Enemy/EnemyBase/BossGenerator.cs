using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGenerator : MonoBehaviour
{
    [Tooltip("���ꂽ�G�I�u�W�F�N�g�͂����ɏ��������B")]
    public GameObject Boss;
    [Tooltip("���b�㏢������邩�����߂�")]
    public float waitSecond;

    //�����֘A
    bool summon = true;
    //�����ʒu
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
