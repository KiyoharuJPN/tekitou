using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;//UniTask

public class MapLine : MonoBehaviour
{
    [SerializeField, Header("マップライン")]
    GameObject[] linePoint;
    [SerializeField, Header("マップラインPoint出現時間")]
    float appTime;
    [SerializeField, Header("マップライン出現間隔")]
    int appInterval;

    [SerializeField, Header("マップ画像")]
    GameObject mapImage;
    [SerializeField, Header("マップ画像出現時間")]
    float popTime;

    //マップ出現演出
    public async void MapFirstSet(StageSelectScene selectScene)
    {
        await UniTask.Delay(500);
        for (int i = 0; i < linePoint.Length; i++)
        {
            _ = linePoint[i].transform.DOScale(
                Vector3.one,
                appTime
                );
            await UniTask.Delay(appInterval);
        }
        await mapImage.transform.DOScale(
                Vector3.one,
                popTime
                );
        selectScene.IsEvent = false;
    }

    //フェード中にマップ配置
    public void MapSet()
    {
        for (int i = 0; i < linePoint.Length; i++)
        {
            linePoint[i].transform.localScale = Vector3.one;
        }
        mapImage.transform.localScale = Vector3.one;
    }
}
