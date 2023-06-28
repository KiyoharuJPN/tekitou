using UnityEngine.UI;
using UnityEngine;

public class InputDebug : MonoBehaviour
{
    //表示用テキスト
    public Text LeftData;
    public Text RightData;
    public Text JumpData;
    public Text AttackData;
    public Text YokoAttackData;
    public Text UpAttackData;
    public Text DownAttackData;
    public Text ExSkillData;

    private void Update()
    {
        CheckKeyEveryFrame();
    }

    public void CheckKeyEveryFrame()
    {
        LeftData.text = AKey().ToString() + "/" + Lsh().ToString();
        RightData.text = DKey().ToString() + "/" + Lsh().ToString();
        JumpData.text = SpaceKey().ToString() + "/" + JumpKey().ToString();
        AttackData.text = UKey().ToString() + "/" + UsualAttackKey().ToString();
        YokoAttackData.text = JKey().ToString() + "/" + YokoKey().ToString();
        UpAttackData.text = IKey().ToString() + "/" + JogeKey().ToString();
        DownAttackData.text = KKey().ToString() + "/" + JogeKey().ToString();
        ExSkillData.text = PKey().ToString() + "/" + LBKey().ToString() + "+" + RBKey().ToString();
    }

    bool AKey() { return Input.GetKey(KeyCode.A); }

    bool DKey() { return Input.GetKey(KeyCode.D); }

    float Lsh() { return Input.GetAxis("L_Stick_H"); }

    bool SpaceKey() { return Input.GetKey(KeyCode.Space); }

    bool JumpKey() { return Input.GetKey("joystick button 0"); }

    bool UKey() { return Input.GetKey(KeyCode.U); }

    bool UsualAttackKey() { return Input.GetKey("joystick button 2"); }

    bool JKey() { return Input.GetKey(KeyCode.J); }

    float YokoKey() { return Input.GetAxis("R_Stick_H"); }

    bool IKey() { return Input.GetKey(KeyCode.I); }

    bool KKey() { return Input.GetKey(KeyCode.K); }

    float JogeKey() { return Input.GetAxis("R_Stick_V"); }

    bool PKey() { return Input.GetKey(KeyCode.P); }

    bool LBKey() { return Input.GetKey(KeyCode.JoystickButton4); }

    bool RBKey() { return Input.GetKey(KeyCode.JoystickButton5); }
}
