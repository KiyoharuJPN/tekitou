using Steamworks;
using UnityEngine;

public class Accmplisment : MonoBehaviour
{
    public readonly static Accmplisment Instance = new();
    void Start()
    {
        if (SteamManager.Initialized)
        {
            //初期化が成功したらAppIDを表示(480はサンプルのID)
            Debug.Log($"Steamの初期化成功, AppID : {SteamUtils.GetAppID()}");
        }
        else
        {
            Debug.Log("Steamの初期化失敗");
        }
    }

    /// <summary>
    /// 実績解除
    /// </summary>
    /// <param name="api">api名</param>
    public void AchvOpen(string api)
    {
        SteamUserStats.SetAchievement(api);
    }
}
