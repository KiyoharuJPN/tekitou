using Steamworks;
using UnityEngine;

public class Accmplisment : MonoBehaviour
{
    public readonly static Accmplisment Instance = new();
    void Start()
    {
        if (SteamManager.Initialized)
        {
            //‰Šú‰»‚ª¬Œ÷‚µ‚½‚çAppID‚ğ•\¦(480‚ÍƒTƒ“ƒvƒ‹‚ÌID)
            Debug.Log($"Steam‚Ì‰Šú‰»¬Œ÷, AppID : {SteamUtils.GetAppID()}");
        }
        else
        {
            Debug.Log("Steam‚Ì‰Šú‰»¸”s");
        }
    }

    /// <summary>
    /// ÀÑ‰ğœ
    /// </summary>
    /// <param name="api">api–¼</param>
    public void AchvOpen(string api)
    {
        SteamUserStats.SetAchievement(api);
    }
}
