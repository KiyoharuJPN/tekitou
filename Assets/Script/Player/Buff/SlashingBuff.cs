using UnityEngine;
using System.Collections;
using static PBF.PlayerBuffBase;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class SlashingBuff : MonoBehaviour
{
    PBF.PlayerBuffBase.SlashingBuff slashing;
    float buffTime;

    SpriteGlow.SpriteGlowEffect spriteGlow;
    //�o�t���Ɍ���F
    Color32 color = Color.green;//��

    //�a���̃X�s�[�h
    float waveSpeed;

    //�c�莞�Ԃ̃o�[
    GameObject timeBar;
    Image timeBarImg;
    float time;

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
        spriteGlow = gameObject.GetComponent<SpriteGlow.SpriteGlowEffect>();

        //�c�莞�Ԃ̃o�[�\���E�ݒ�
        timeBar = GameObject.Find("PlayerBuffTime");
        timeBar.GetComponent<UIPosController>().enabled = true;
        timeBar.GetComponent<Canvas>().enabled = true;
        timeBarImg = timeBar.transform.Find("Bar").GetComponent<Image>();

        waveSpeed = slashing.slashingSpeed;

        if (!gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.GlowColor = color;
        }

        StartCoroutine(SlashingMode());
    }

    //�a���������\�b�h
    public void Slashing(SlashingType type, GameObject player)
    {
        switch (type)
        {
            case SlashingType.sideAttack_Right:
                SlashingWaveGenerate(player.transform.position + new Vector3(1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), false, new Vector2(waveSpeed, 0));
                break;
            case SlashingType.sideAttack_Left:
                SlashingWaveGenerate(player.transform.position + new Vector3(-1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), true, new Vector2(-waveSpeed, 0));
                break;
            case SlashingType.UpAttack:
                SlashingWaveGenerate(player.transform.position + new Vector3(0, 1f, 0),
                    Quaternion.Euler(0f, 0f, 90f), false, new Vector2(0, waveSpeed));
                break;
            case SlashingType.DropAttack:
                SlashingWaveGenerate(player.transform.position + new Vector3(1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), false, new Vector2(waveSpeed, 0));
                SlashingWaveGenerate(player.transform.position + new Vector3(-1f, 0, 0),
                    Quaternion.Euler(0f, 0f, 0f), true, new Vector2(-waveSpeed, 0));
                break;
        }

    }

    //�a���g����(������:�����ꏊ�A������:�p�x�A��O����:X���]�̗L���A��l����:��)
    private void SlashingWaveGenerate(Vector3 position, Quaternion rotation, bool flipX, Vector2 velocity)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.SlashingWave);
        GameObject obj = Instantiate(slashing.slashingObj, position, Quaternion.identity);
        obj.transform.rotation = rotation;
        obj.GetComponent<SlashingWave>().player = this.gameObject.GetComponent<PlayerController>();

        obj.GetComponent<SpriteRenderer>().flipX = flipX;

        obj.GetComponent <Rigidbody2D>().velocity = velocity;
    }

    internal void AddBuff(int count)
    {
        timeBar = GameObject.Find("PlayerBuffTime");
        float addTime = slashing.buffSetTime - slashing.buffTimeDown * count;
        buffTime += addTime;
        time += addTime;

        //�o�[�ăZ�b�g
        timeBarImg.fillAmount = 1;
        timeBarImg.fillAmount -= 1 - (time / buffTime);
    }

    IEnumerator SlashingMode()
    {
        timeBarImg.fillAmount = 1;
        time = buffTime;

        while(time > 0)
        {
            if (this.gameObject.GetComponent<PlayerController>().canMove)
            {
                time -= Time.deltaTime;
                timeBarImg.fillAmount -= Time.deltaTime / buffTime;
            }
            yield return null;
        };

        if (gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.GlowColor = Color.cyan;
        }
        if(!gameObject.GetComponent<SpeedUp>() && !gameObject.GetComponent<InvinciblBuff>())
        {
            spriteGlow.EnableInstancing = false;
        }

        timeBar.GetComponent<UIPosController>().enabled = false;
        timeBar.GetComponent<Canvas>().enabled = false;

        PlayerBuff.Instance.CountReset_Slashing();
        Destroy(this.GetComponent<SlashingBuff>());
    }
}
