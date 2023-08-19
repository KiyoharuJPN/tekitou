using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPosController : MonoBehaviour
{
    [SerializeField]
    private Transform targetTfm;

    private RectTransform myRectTfm;
    private Vector3 offset = new Vector3(0, 1.5f, 0);

    void Start()
    {
        myRectTfm = GetComponent<RectTransform>();
    }

    void Update()
    {
        myRectTfm.position = targetTfm.position + offset;
        PlayerBuff.Instance.GetBuffCount(PBF.PlayerBuffBase.BuffType.Invincible);
    }
}
