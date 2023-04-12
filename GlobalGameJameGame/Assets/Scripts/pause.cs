using UnityEngine;

public class pause : MonoBehaviour
{
    public GameObject obj,showobj,hideobj;

    private void Update()
    {
        if (obj.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            showobj.SetActive(true);
            hideobj.SetActive(false);
        }
    }
}
