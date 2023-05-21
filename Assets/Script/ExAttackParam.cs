using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExAttackParam : MonoBehaviour
{
    [Header("vC[")]
    [SerializeField]
    PlayerController player;

    [Header("Q[WΚ")]
    [SerializeField]
    int gauge;

    //TODO@C³\θ
    [Header("Q[WΜζ")]
    [SerializeField]
    Image exGauge;
    [SerializeField]
    GameObject exAttackText;

    private bool isExAttack;

    public bool GetIsExAttack
    {
        get { return isExAttack; }
    }

    int _exAttack;

    public static ExAttackParam Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        exAttackText.GetComponent<Image>().enabled = false;
        exGauge.fillAmount = 0f;
    }

    private void FixedUpdate()
    {
        if(gauge == _exAttack)
        {
            exAttackText.GetComponent<Image>().enabled = true;
        }
    }

    public void AddGauge()
    {
        Debug.Log("KEZQ[Wͺ­άΑ½");
        exGauge.fillAmount += 1f / gauge;
        _exAttack++;
    }
}
