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
        // �����ʒu��ێ�
        
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
    /// �h��J�n
    /// </summary>
    /// <param name="swingWigth">�ő�h�ꕝ</param>
    /// <param name="duration">����</param>
    /// <param name="vibrato">�U����</param>
    /// <param name="randomness">�U������͈�</param>
    public void StartShake(Vector2 swingWigth, float duration, int vibrato, float randomness)
    {
        _initPosition = transform.position;
        // �O��̏������c���Ă���Β�~���ď����ʒu�ɖ߂�
        if (_shakeTweener != null)
        {
            _shakeTweener.Kill();
            gameObject.transform.position = _initPosition;
        }
        // �h��J�n
        _shakeTweener = gameObject.transform.DOPunchPosition(swingWigth, duration, vibrato, randomness);
    }
}
