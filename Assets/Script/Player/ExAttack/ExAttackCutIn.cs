using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExAttackCutIn : MonoBehaviour
{
    //カットイン状態
    private bool isCutIn = false;

    public bool GetIsCutIn
    {
        get { return isCutIn; }
    }

    public static ExAttackCutIn Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CutInAnime_End()
    {
        isCutIn = false;
    }

    public IEnumerator _ExAttackCutIn(PlayerController player)
    {
        
        this.GetComponent<Animator>().SetTrigger("ExAttack");
        isCutIn = true;

        while (!isCutIn)
        {
            //1フレーム待つ
            yield return null;
        }
        player.ExAttackStart();
    }

    //カットインSE
    public void _ExAttack_CutIn()
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.ExAttack_CutIn);
    }

}
