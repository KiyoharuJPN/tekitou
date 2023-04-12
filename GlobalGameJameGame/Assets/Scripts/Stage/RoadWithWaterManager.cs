using UnityEngine;

public class RoadWithWaterManager : MonoBehaviour
{
    GameObject player;
    DarkController darkController;
    public GameObject[] shobj;

    private void Start()
    {
        player = GameObject.Find("Player");
        darkController = player.GetComponentInChildren<DarkController>();
    }

    private void Update()
    {
        if (darkController.GetPlayerLighting() && !shobj[0].activeSelf)
        {
            for (int i = 0; i < shobj.Length; i++)
            {
                shobj[i].SetActive(true);
            }
        }
        else if (!darkController.GetPlayerLighting() && shobj[0].activeSelf)
        {
            for (int i = 0; i < shobj.Length; i++)
            {
                shobj[i].SetActive(false);
            }
        }
    }
}
