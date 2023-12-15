using UnityEngine.UI;

//デバッグ用操作・情報確認スクリプト
public class DebugState : DebugBase
{

    //プレイヤーポジション表示用テキスト
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
            isAttack.text = "攻撃中";
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
            return "可能";
        }
        else
        {
            return "不可";
        }
    }
}
