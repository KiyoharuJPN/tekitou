using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [System.Serializable]
    public struct ItemData
    {
        [Tooltip("スコア")]
        public int score;
        [Tooltip("回復量")]
        public int resilience;
    }

    [SerializeField]
    [Header("アイテム詳細")]
    public ItemData itemData = new ItemData { score = 0, resilience = 0 };

    Collider2D col;

    private void Start()
    {
        
    }

    virtual protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("InvinciblePlayer"))
        {
            Destroy(this.gameObject);
        }
    }
}
