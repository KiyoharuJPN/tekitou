using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kya : MonoBehaviour
{

    GameObject kyaru;
    bool check = true;

    private void Awake()
    {
        kyaru = GameObject.Find("Square");
        StartCoroutine(AccessHelloWorld());
    }



    IEnumerator AccessHelloWorld()
    {
        Debug.Log(kyaru);
        kyaru.GetComponent<Ru>().HelloWorld();

        yield return new WaitForSeconds(2);

        Debug.Log(kyaru);
        Debug.Log(kyaru.GetComponent<Ru>());
        kyaru.GetComponent<Ru>().HelloWorld();
    }

}
