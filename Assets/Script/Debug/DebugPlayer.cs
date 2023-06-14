using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugPlayer : DebugBase
{
    //デバッグメニューCanvas
    [SerializeField]
    internal Canvas debugCanvas;

    bool canEnabled = true;
    private void Update()
    {
        ControllerKeyBoard();
        CheatMane();
    }

    //プレイヤー操作（キーボード）
    void ControllerKeyBoard()
    {
        //横移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            p_Walk.moveInput = -1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            p_Walk.moveInput = 1;
        }

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            p_Jump.JumpSet();
        }

        //攻撃
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

    //チートメニュー
    void CheatMane()
    {
        //Exゲージチャージ
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ExAttackParam.Instance.SetGage(50);
        }

        //体力満タン
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            p_Controller._Heel(6);
        }

        //シーンリロード
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //コンボゲージ関係
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

        //デバッグ操作キー確認メニュー表示非表示
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

        //タイトルへ
        if (SceneManager.GetActiveScene().name != "Level_Testing" && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }
    }
}
