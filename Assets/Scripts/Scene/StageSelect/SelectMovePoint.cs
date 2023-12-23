using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectMovePoint : MonoBehaviour
{
    [SerializeField, Header("ステージ名")]
    private string stageName;

    //遊べるポイントかどうか
    private bool isPlayPoint;

    private Tweener _shakeTweener;
    private Vector2 _initPosition;
    private SpriteRenderer m_Image;

    private void Awake()
    {
        // 初期位置を保持
        _initPosition = transform.position;
        m_Image = GetComponent<SpriteRenderer>();
    }
    public bool IsPlayPoint { get { return isPlayPoint; } set { isPlayPoint = value; } }
    public Vector2 GetPos => _initPosition;
    public string GetStageName => stageName;
    public SpriteRenderer PointImage { 
        get
        {
            return m_Image;
        }
        set { m_Image = value; }
    }

    /// <summary>
    /// 揺れ開始
    /// </summary>
    /// <param name="swingWigth">最大揺れ幅</param>
    /// <param name="duration">時間</param>
    /// <param name="vibrato">振動数</param>
    /// <param name="randomness">振動する範囲</param>
    public void StartShake(Vector2 swingWigth, float duration, int vibrato, float randomness)
    {
        // 前回の処理が残っていれば停止して初期位置に戻す
        if (_shakeTweener != null)
        {
            _shakeTweener.Kill();
            gameObject.transform.position = _initPosition;
        }
        // 揺れ開始
        _shakeTweener = gameObject.transform.DOPunchPosition(swingWigth, duration, vibrato, randomness);
    }
}
