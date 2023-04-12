using UnityEngine.SceneManagement;
using UnityEngine;

public class ClickToChangeScene : MonoBehaviour
{
    public string MoveScene = "";

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(MoveScene != "")
            {
                SceneManager.LoadScene(MoveScene);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    
}
