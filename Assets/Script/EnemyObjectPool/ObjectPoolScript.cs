using UnityEngine.SceneManagement;
using UnityEngine;

public class ObjectPoolScript : MonoBehaviour
{
    string SceneNamePre;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SceneNamePre != SceneManager.GetActiveScene().name)
        {
            SceneNamePre = SceneManager.GetActiveScene().name;
            if (SceneNamePre == "Title")
            {
                Destroy(gameObject);
            }
            else
            {
                
            }


        }
        
    }

    public void SceneReset()
    {
        
        Wizard_MagicBall[] wmbChildren = transform.GetComponentsInChildren<Wizard_MagicBall>();
        if (wmbChildren != null)
        {
            foreach (Wizard_MagicBall child in wmbChildren)
            {
                if (child.gameObject.activeSelf)
                {
                    child.RestoreToPool();
                }
            }
        }
    }
}
