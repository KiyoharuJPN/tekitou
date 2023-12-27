using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;//UniTask

public class MapLine : MonoBehaviour
{
    [SerializeField, Header("�}�b�v���C��")]
    GameObject[] linePoint;
    [SerializeField, Header("�}�b�v���C��Point�o������")]
    float appTime;
    [SerializeField, Header("�}�b�v���C���o���Ԋu")]
    int appInterval;

    [SerializeField, Header("�}�b�v�摜")]
    GameObject mapImage;
    [SerializeField, Header("�}�b�v�摜�o������")]
    float popTime;

    //�}�b�v�o�����o
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

    //�t�F�[�h���Ƀ}�b�v�z�u
    public void MapSet()
    {
        for (int i = 0; i < linePoint.Length; i++)
        {
            linePoint[i].transform.localScale = Vector3.one;
        }
        mapImage.transform.localScale = Vector3.one;
    }
}
