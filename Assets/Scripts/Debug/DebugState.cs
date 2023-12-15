using UnityEngine.UI;

//�f�o�b�O�p����E���m�F�X�N���v�g
public class DebugState : DebugBase
{

    //�v���C���[�|�W�V�����\���p�e�L�X�g
    public Text posText_X;
    public Text posText_Y;
    public Text isAttack;
    public Text canNomalAttack;
    public Text canUpAttack;
    public Text canDropAttack;
    public Text canSideAttack;
    public Text canExAttack;


    private void FixedUpdate()
    {
        PlayerState();
    }

    void PlayerState()
    {
        posText_X.text = p_obj.transform.position.x.ToString("f5");
        posText_Y.text = p_obj.transform.position.y.ToString("f5");
        if (p_Controller.isAttack)
        {
            isAttack.text = "�U����";
        }
        else { isAttack.text = "No"; }

        canNomalAttack.text = IsState(p_Controller.canNomalAttack);
        canUpAttack.text = IsState(p_Controller.canUpAttack);
        canDropAttack.text = IsState(p_Controller.canDropAttack);
        canSideAttack.text = IsState(p_Controller.canSideAttack);
        canExAttack.text = IsState(p_Controller.canExAttack);
    }

    string IsState(bool canAttack)
    {
        if (canAttack)
        {
            return "�\";
        }
        else
        {
            return "�s��";
        }
    }
}
