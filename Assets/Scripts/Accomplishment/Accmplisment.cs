using Steamworks;
using UnityEngine;

public class Accmplisment : MonoBehaviour
{
    public readonly static Accmplisment Instance = new();
    void Start()
    {
        
    }

    public bool GameStart()
    {
        if (SteamManager.Initialized)
        {
            //������������������AppID��\��(480�̓T���v����ID)
            Debug.Log($"Steam�̏���������, AppID : {SteamUtils.GetAppID()}");
            return true;
        }
        else
        {
            Debug.Log("Steam�̏��������s");
            return false;
        }
    }

    /// <summary>
    /// ���щ���
    /// </summary>
    /// <param name="api">api��</param>
    public void AchvOpen(string api)
    {
        SteamUserStats.SetAchievement(api);
        SteamUserStats.StoreStats();
    }
}
