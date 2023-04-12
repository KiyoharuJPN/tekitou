using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GameObject hideobj;
    public GameObject showobj;
    public void Continue()
    {
        hideobj.SetActive(false);
    }

    public void Settings()
    {
        hideobj.SetActive(false);
        showobj.SetActive(true);
    }
}
