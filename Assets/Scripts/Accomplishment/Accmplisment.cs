using Steamworks;
using UnityEngine;

public class Accmplisment : MonoBehaviour
{
    public readonly static Accmplisment Instance = new();
    void Start()
    {
        if (SteamManager.Initialized)
        {
            //������������������AppID��\��(480�̓T���v����ID)
            Debug.Log($"Steam�̏���������, AppID : {SteamUtils.GetAppID()}");
        }
        else
        {
            Debug.Log("Steam�̏��������s");
        }
    }

    /// <summary>
    /// ���щ���
    /// </summary>
    /// <param name="api">api��</param>
    public void AchvOpen(string api)
    {
        SteamUserStats.SetAchievement(api);
    }
}
