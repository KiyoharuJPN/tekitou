using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGool : MonoBehaviour
{
    [SerializeField] FadeImage fade;
    [SerializeField] new CameraManager camera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player") 
        {
            StartCoroutine(Result(collision));
        }
    }

    IEnumerator Result(Collider2D player)
    {
        player.GetComponent<PlayerController>().SetCanMove(false);

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
