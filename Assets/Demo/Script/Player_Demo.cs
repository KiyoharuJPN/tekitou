using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Demo : PlayerController
{
    public bool playerOpe = false;

    protected override void InputKay()
    {
        if (playerOpe)
        {
            DemoInputKey();
        }
        else if (!playerOpe)
        {
            NomalInputKay();
        }
    }

    void NomalInputKay()
    {
        var inputMoveAxis = move.ReadValue<Vector2>();

        if (nomalAttack.IsPressed())
        {
            isAttackKay = true;
        }
        else { isAttackKay = false; }
        if (skillAttack.IsPressed())
        {
            isSkillAttackKay = true;
        }
        else { isSkillAttackKay = false; }

        //�㏸�U��
        //if (inputMoveAxis.y >= 0.9 && isSkillAttackKay || Input.GetKey(KeyCode.I))
        if (inputMoveAxis.y >= 0.9 && isSkillAttackKay)
        //rsv >= 0.8
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (inputMoveAxis.y <= -0.9 && isSkillAttackKay && canDropAttack)
        //rsv <= -0.8
        {
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        //if (inputMoveAxis.x >= 0.9 && isSkillAttackKay || Input.GetKey(KeyCode.K))
        if (inputMoveAxis.x >= 0.9 && isSkillAttackKay)
        {
            AttackAction("SideAttack_right");
        }
        //else if (inputMoveAxis.x <= -0.9 && isSkillAttackKay || Input.GetKey(KeyCode.J))
        else if (inputMoveAxis.x <= -0.9 && isSkillAttackKay)
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed())
        {
            if (!isAttack && canExAttack)
            {
                AttackAction("ExAttack");
            }
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (nomalAttack.WasPressedThisFrame() && canNomalAttack)
        {
            //�ʏ�U������
            AttackAction("NomalAttack");
        }
        //if (nomalAttack.IsPressed() && canNomalAttack || Input.GetKey(KeyCode.U))
        if (nomalAttack.IsPressed() && canNomalAttack)
        {
            //�ʏ�U����������
            AttackAction("NomalAttack");
        }
    }

    void DemoInputKey()
    {
        var inputMoveAxis = move.ReadValue<Vector2>();

        if (nomalAttack.IsPressed())
        {
            isAttackKay = true;
        }
        else { isAttackKay = false; }

        //�㏸�U��
        if (inputMoveAxis.y >= 0.9 && !(inputMoveAxis.x >= 0.5 || inputMoveAxis.x <= -0.5) && isAttackKay)
        //rsv >= 0.8
        {
            AttackAction("UpAttack");
        }
        //�����U���U��
        if (inputMoveAxis.y <= -0.9 && isAttackKay)
        //rsv <= -0.8
        {
            AttackAction("DawnAttack");
        }
        //���ړ��U��
        if (inputMoveAxis.x >= 0.9 && isAttackKay)
        {
            AttackAction("SideAttack_right");
        }
        else if (inputMoveAxis.x <= -0.9 && isAttackKay)
        {
            AttackAction("SideAttack_left");
        }
        //�K�E�Z
        if (exAttack_L.IsPressed() && exAttack_R.IsPressed())
        {
            if (!isAttack && canExAttack)
            {
                AttackAction("ExAttack");
            }
        }
        //�蓮�U���F�U���{�^���������ꂹ���Ƃ�
        if (nomalAttack.WasPressedThisFrame() && canNomalAttack)
        {
            //�ʏ�U������
            AttackAction("NomalAttack");
        }
        if (nomalAttack.IsPressed() && canNomalAttack)
        {
            //�ʏ�U����������
            AttackAction("NomalAttack");
        }
    }

    public override void _Damage(int power)
    {
        if (gameObject.GetComponent<InvinciblBuff>()) { return; }
        if (!inInvincibleTimeLife)
        {
            //���G���Ԃ̌v�Z
            inInvincibleTimeLife = true;
            StartCoroutine(InvincibleLife());

            //���C�t�v�Z
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
