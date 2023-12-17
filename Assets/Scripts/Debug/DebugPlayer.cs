using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugPlayer : DebugBase
{
    //�f�o�b�O���j���[Canvas
    [SerializeField]
    internal Canvas debugCanvas;

    bool canEnabled = true;

    bool canTimeScele = false;
    private void Update()
    {
        ControllerKeyBoard();
        CheatMane();
    }

    //�v���C���[����i�L�[�{�[�h�j
    void ControllerKeyBoard()
    {
        //���ړ�
        if (Input.GetKeyDown(KeyCode.A))
        {
            p_Walk.moveInput = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            p_Walk.moveInput = 1;
        }

        //�W�����v
        if (Input.GetKeyDown(KeyCode.Space))
        {
            p_Jump.JumpSet();
        }

        //�U��
        if (Input.GetKeyDown(KeyCode.U))
        {
            p_Controller.AttackAction("NomalAttack");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Input.GetKey(KeyCode.A)) p_Controller.AttackAction("SideAttack_left");
            if (Input.GetKey(KeyCode.D)) p_Controller.AttackAction("SideAttack_right");
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            p_Controller.AttackAction("UpAttack");
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            p_Controller.AttackAction("DawnAttack");
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            p_Controller.AttackAction("ExAttack");
        }
    }

    //�`�[�g���j���[
    void CheatMane()
    {
        //Ex�Q�[�W�`���[�W
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExAttackParam.Instance.SetGage(50);
        }

        //�̗͖��^��
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            p_Controller.Heel(6);
        }

        //�V�[�������[�h
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //�R���{�Q�[�W�֌W
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() - 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() - 10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ComboParam.Instance.SetCombo(ComboParam.Instance.GetCombo() + 10);
        }

        //�f�o�b�O����L�[�m�F���j���[�\����\��
        if (Input.GetKeyDown(KeyCode.Alpha0) && canEnabled)
        {
            canEnabled = false;
            if(debugCanvas.enabled == false)
            {
                debugCanvas.enabled = true;
            }
            else if (debugCanvas.enabled == true)
            {
                debugCanvas.enabled = false;
            }
            canEnabled = true;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SceneData.Instance.referer = "Stage2";
            SceneManager.LoadScene("Load");
        }

        //���x�o�t�t�^
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerBuff.Instance.ExAttackGageUp();
            PlayerBuff.Instance.SpeedUp();
            PlayerBuff.Instance.SlashingBuff();
            PlayerBuff.Instance.InvincibleBuff();
            Debug.Log("�o�t�t�^");
        }
        //�o�t���Z�b�g
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerBuff.Instance.BuffRest();
        }


        //�^�C�g����
        if (SceneManager.GetActiveScene().name != "Level_Testing" && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }
    }
}