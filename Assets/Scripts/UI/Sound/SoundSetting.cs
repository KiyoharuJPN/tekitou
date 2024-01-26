using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundSetting : MonoBehaviour, MenuSystem
{
    [SerializeField] public PauseMenu BackMenu;
    [SerializeField, Header("トライアングルポインター")]
    private GameObject target;

    internal enum SelectMenu
    {
        SE = 0,
        BGM = 1,
        RESET = 2,
        BACK = 3,
    }
    internal SelectMenu selectMenu = 0;

    public TextMeshProUGUI[] menuObj;            //メニュー画面のオブジェクト

    [SerializeField] internal SliderBase BGMSlider;
    [SerializeField] internal SliderBase SESlider;
    internal SliderBase selectSlider;

    const float defaultValue = 0.5f;

    //InputSystem
    public InputAction back, decision, move;
    private bool isPointerMove = true;

    public float sliderMoveSpeed = 0.1f;
    //メニュー表示確認Bool
    internal bool isPauseMenu = false;
    private Color color = new Color(255, 69, 0);

    public virtual void InputSet(PlayerInput playerInput)
    {
        isPauseMenu = true;
        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        GameManager.Instance.SetMenu(this);

        OnSelected((int)selectMenu);
    }

    public bool PauseCheck()
    {
        return isPauseMenu;
    }

    public virtual void MenuUpdata()
    {
        if(selectSlider == null)
        {
            StickerChangePointer();
        }
        else if(selectSlider != null)
        {
            ChangeSlider();
        }

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
             SelectMenuProcess();
        }

        if (back.WasPressedThisFrame())
        {
            GameManager.Instance.MenuBack();
        }
    }

    internal void StickerChangePointer()
    {
        var input = move.ReadValue<Vector2>().y;

        if(input > 0.3f && (int)selectMenu > 0 && isPointerMove)
        {
            isPointerMove = false;
            ChangePointer(-1);
        }
        if(input < -0.3f && (int)selectMenu < 3 && isPointerMove)
        {
            isPointerMove = false;
            ChangePointer(1);
        }
        if(input == 0)
        {
            isPointerMove = true;
        }
    }

    internal void ChangeSlider()
    {
        var input = move.ReadValue<Vector2>().x;

        if (input != 0)
        {
            var value = selectSlider.GetSliderValue + (input * sliderMoveSpeed) * Time.unscaledDeltaTime * 1000;
            selectSlider.SetValue(value);

            switch (selectMenu)
            {
                case SelectMenu.SE:
                    SoundManager.Instance.SetSEVolume = value; break;
                case SelectMenu.BGM:
                    SoundManager.Instance.SetBGMVolume = value; break;
            }
        }
    }

    void ChangePointer(int pointer)
    {
        OnDeselected((int)selectMenu);
        selectMenu += pointer;
        OnSelected((int)selectMenu);
    }

    internal virtual void SelectMenuProcess()
    {
        
        switch (selectMenu)
        {
            case SelectMenu.BGM:
                selectSlider = BGMSlider;
                selectSlider.Active(true);
                break;
            case SelectMenu.SE:
                selectSlider = SESlider;
                selectSlider.Active(true);
                break;
            case SelectMenu.RESET: //音量を初期値に変更
                SoundValueReset();
                break;
            case SelectMenu.BACK:
                GameManager.Instance.MenuBack();
                break;
        }
    }

    internal void SoundValueReset()
    {
        BGMSlider.SetValue(defaultValue);
        SESlider.SetValue(defaultValue);

        SoundManager.Instance.SetSEVolume = defaultValue;
        SoundManager.Instance.SetBGMVolume = defaultValue;
    }

    public virtual MenuSystem Back()
    {
        if(selectSlider != null)
        {
            selectSlider.Active(false);
            selectSlider = null;

            return this;
        }
        else
        {
            isPauseMenu = false;
            this.gameObject.SetActive(false);

            return BackMenu;
        }
    }

    internal void OnSelected(int objNum)
    {
        if (objNum > 1)
        {
            target.SetActive(true);
            target.transform.position = new Vector2(target.transform.position.x, menuObj[objNum].transform.position.y);
        }
        else if(objNum < 2)
        {
            target.SetActive(false);
        }
        
        menuObj[objNum].color = color;    //UIの色変更
    }
    internal void OnDeselected(int objNum)
    {
        menuObj[objNum].color = Color.white; //色を戻す
    }
}
