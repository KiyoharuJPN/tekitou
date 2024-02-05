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
            //初期化が成功したらAppIDを表示(480はサンプルのID)
            Debug.Log($"Steamの初期化成功, AppID : {SteamUtils.GetAppID()}");
            isInitialized = true;
            return true;
        }
        else
        {
            Debug.Log("Steamの初期化失敗");
            isInitialized = false;
            return false;
        }
    }

    /// <summary>
    /// 実績解除
    /// </summary>
    /// <param name="api">api名</param>
    public void AchvOpen(string api)
    {
        if (isInitialized)
        {
            SteamUserStats.SetAchievement(api);
            SteamUserStats.StoreStats();
        }
    }
}
