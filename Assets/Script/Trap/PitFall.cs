using UnityEngine;
using UnityEngine.SceneManagement;

public class PitFall : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
