using UnityEngine.SceneManagement;
using UnityEngine;

public class ObjectPoolScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            Destroy(gameObject);
        }
    }
}
