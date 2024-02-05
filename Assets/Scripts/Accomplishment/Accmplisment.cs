using Steamworks;
using UnityEngine;

public class Accmplisment : MonoBehaviour
{
    public readonly static Accmplisment Instance = new();

    private bool isInitialized = false;

    public bool GameStart()
    {
        if (SteamManager.Initialized)
        {
            //������������������AppID��\��(480�̓T���v����ID)
            Debug.Log($"Steam�̏���������, AppID : {SteamUtils.GetAppID()}");
            isInitialized = true;
            return true;
        }
        else
        {
            Debug.Log("Steam�̏��������s");
            isInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// ���щ���
    /// </summary>
    /// <param name="api">api��</param>
    public void AchvOpen(string api)
    {
        if (isInitialized)
        {
            SteamUserStats.SetAchievement(api);
            SteamUserStats.StoreStats();
        }
    }
}
