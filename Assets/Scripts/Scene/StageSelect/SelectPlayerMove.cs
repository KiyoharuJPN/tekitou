using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SelectPlayerMove : MonoBehaviour
{
    private Tweener _shakeTweener;
    private Vector2 _initPosition;
    private SpriteRenderer m_Image;

    private void Awake()
    {
        // ‰ŠúˆÊ’u‚ğ•Û
        
        m_Image = GetComponent<SpriteRenderer>();
    }
    public Vector2 GetPos => _initPosition;
    public SpriteRenderer PointImage
    {
        get
        {
            return m_Image;
        }
        set { m_Image = value; }
    }

    /// <summary>
    /// —h‚êŠJn
    /// </summary>
    /// <param name="swingWigth">Å‘å—h‚ê•</param>
    /// <param name="duration">ŠÔ</param>
    /// <param name="vibrato">U“®”</param>
    /// <param name="randomness">U“®‚·‚é”ÍˆÍ</param>
    public void StartShake(Vector2 swingWigth, float duration, int vibrato, float randomness)
    {
        _initPosition = transform.position;
        // ‘O‰ñ‚Ìˆ—‚ªc‚Á‚Ä‚¢‚ê‚Î’â~‚µ‚Ä‰ŠúˆÊ’u‚É–ß‚·
        if (_shakeTweener != null)
        {
            _shakeTweener.Kill();
            gameObject.transform.position = _initPosition;
        }
        // —h‚êŠJn
        _shakeTweener = gameObject.transform.DOPunchPosition(swingWigth, duration, vibrato, randomness);
    }
}
