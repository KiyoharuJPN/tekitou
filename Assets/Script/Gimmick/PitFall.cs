using UnityEngine;
using UnityEngine.SceneManagement;

public class PitFall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
            GameManager.Instance.PlayerDeath();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
