using UnityEngine;
using System.Collections;
using static PBF.PlayerBuffBase;
using System;

public class SlashingBuff : MonoBehaviour
{
    slashingBuff slashing;
    float buffTime;

    public enum SlashingType
    {
        sideAttack_Right,
        sideAttack_Left,
        UpAttack,
        DropAttack
    }

    void Start()
    {
        slashing = PlayerBuff.Instance.GetSlashing();
        buffTime = slashing.firstSetTime;
        StartCoroutine(SlashingMode());
    }

    //�a���������\�b�h
    public void Slashing(SlashingType type, GameObject player)
    {
        GameObject obj;
        switch (type)
        {
            case SlashingType.sideAttack_Right:
                obj = Instantiate(slashing.slashingObj, player.transform.position + new Vector3(1f,0,0), Quaternion.identity);
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(PlayerBuff.Instance.GetPlayerMoveData(),0);
                break;
            case SlashingType.sideAttack_Left:
                obj = Instantiate(slashing.slashingObj, player.transform.position + new Vector3(-1f, 0, 0), Quaternion.identity);
                obj.GetComponent<SpriteRenderer>().flipX = true;
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-PlayerBuff.Instance.GetPlayerMoveData(), 0);
                break;
            case SlashingType.UpAttack:
                obj = Instantiate(slashing.slashingObj, player.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(0f, 0f, 90f); ;
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, PlayerBuff.Instance.GetPlayerMoveData());
                break;
            case SlashingType.DropAttack:
                obj = Instantiate(slashing.slashingObj, player.transform.position + new Vector3(1f, 0, 0), Quaternion.identity);
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(PlayerBuff.Instance.GetPlayerMoveData(), 0);
                obj = Instantiate(slashing.slashingObj, player.transform.position + new Vector3(-1f, 0, 0), Quaternion.identity);
                obj.GetComponent<SpriteRenderer>().flipX = true;
                obj.GetComponent<Rigidbody2D>().velocity = new Vector2(-PlayerBuff.Instance.GetPlayerMoveData(), 0);
                break;
        }
    }

    internal void AddBuff(int count)
    {
        buffTime += slashing.buffSetTime - slashing.buffTimeDown * count;
    }

    IEnumerator SlashingMode()
    {
        while(buffTime > 0)
        {
            buffTime -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.GetComponent<SlashingBuff>());
    }
}
