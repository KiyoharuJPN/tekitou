using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string SceneName;
    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ChangeScene();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ChangeScene();
    }
}
