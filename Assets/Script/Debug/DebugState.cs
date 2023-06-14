using UnityEngine.UI;

//�f�o�b�O�p����E���m�F�X�N���v�g
public class DebugState : DebugBase
{

    //�v���C���[�|�W�V�����\���p�e�L�X�g
    public Text posText_X;
    public Text posText_Y;

    private void FixedUpdate()
    {
        PlayerState();
    }

    void PlayerState()
    {
        posText_X.text = p_obj.transform.position.x.ToString("f5");
        posText_Y.text = p_obj.transform.position.y.ToString("f5");
    }
}
