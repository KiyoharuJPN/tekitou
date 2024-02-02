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
            //‰Šú‰»‚ª¬Œ÷‚µ‚½‚çAppID‚ğ•\¦(480‚ÍƒTƒ“ƒvƒ‹‚ÌID)
            Debug.Log($"Steam‚Ì‰Šú‰»¬Œ÷, AppID : {SteamUtils.GetAppID()}");
            return true;
        }
        else
        {
            Debug.Log("Steam‚Ì‰Šú‰»¸”s");
            return false;
        }
    }

    /// <summary>
    /// ÀÑ‰ğœ
    /// </summary>
    /// <param name="api">api–¼</param>
    public void AchvOpen(string api)
    {
        SteamUserStats.SetAchievement(api);
        SteamUserStats.StoreStats();
    }
}
