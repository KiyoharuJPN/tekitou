using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Demo : PlayerController
{
    protected override void InputKay()
    {
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        //ŠÈˆÕ“ü—Í‚Åg—p
        //float rsh = Input.GetAxis("R_Stick_H");
        //float rsv = Input.GetAxis("R_Stick_V");

        if (Input.GetKey(KeyCode.JoystickButton2))
        {
            isAttackKay = true;
        }
        else { isAttackKay = false; }

        //ã¸UŒ‚
        if (lsv >= 0.9 && lsh <= 0.45 && isAttackKay)
        //rsv >= 0.8
        {
            AttackAction("UpAttack");
        }
        //—‰ºUŒ‚UŒ‚
        if (lsv <= -0.9 && lsh <= 0.45 && isAttackKay)
        //rsv <= -0.8
        {
            AttackAction("DawnAttack");
        }
        //‰¡ˆÚ“®UŒ‚
        if (lsh >= 0.9 && lsv <= 0.45 && isAttackKay)
        {
            AttackAction("SideAttack_right");
        }
        else if (lsh <= -0.9 && lsv <= 0.45 && isAttackKay)
        {
            AttackAction("SideAttack_left");
        }
        //•KE‹Z
        if (Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5))
        {
            if (!isAttack && canExAttack)
            {
                AttackAction("ExAttack");
            }
        }
        //è“®UŒ‚FUŒ‚ƒ{ƒ^ƒ“‚ª‰Ÿ‚³‚ê‚¹‚½‚Æ‚«
        if (Input.GetKeyDown(KeyCode.JoystickButton2) && canNomalAttack)
        {
            //’ÊíUŒ‚“ü—Í
            AttackAction("NomalAttack");
        }
        if (Input.GetKey(KeyCode.JoystickButton2) && canNomalAttack)
        {
            //’ÊíUŒ‚’·‰Ÿ‚µ’†
            AttackAction("NomalAttack");
        }
    }

    public override void _Damage(int power)
    {
        if (gameObject.GetComponent<InvinciblBuff>()) { return; }
        if (!inInvincibleTimeLife)
        {
            //–³“GŠÔ‚ÌŒvZ
            inInvincibleTimeLife = true;
            StartCoroutine(InvincibleLife());

            //ƒ‰ƒCƒtŒvZ
            hpparam.DamageHP(hpparam.GetHP() - power);
            shake.Shake(0.2f, 0.8f, true, true);
            if (hpparam.GetHP() <= 0)
            {
                this.tag = "DeadPlayer";
                gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
                isKnockingBack = false;
                SoundManager.Instance.PlaySE(SESoundData.SE.PlayerDead);
                animator.Play("Death");
                shake.Shake(0.2f, 1f, true, true);
                GameManager.Instance.DemoPlayerDeath();
            }
        }
    }
}
