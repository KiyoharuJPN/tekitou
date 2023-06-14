using UnityEngine.UI;

//デバッグ用操作・情報確認スクリプト
public class DebugState : DebugBase
{

    //プレイヤーポジション表示用テキスト
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
