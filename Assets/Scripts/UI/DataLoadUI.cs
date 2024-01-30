using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DataLoadUI : MonoBehaviour, MenuSystem
{
    [SerializeField] public OptionMenu backMenu;
    [SerializeField] public TitleMenu titleMenu;

    [SerializeField, Header("トライアングルポインター")]
    private GameObject target;

    internal enum SelectMenu
    {
        YES = 0,
        NO = 1,
        BACK = 2,
    }
    internal SelectMenu selectMenu = 0;

    public Text[] menuObj;            //メニュー画面のオブジェクト

    //説明文TextBox
    [SerializeField] private TextMeshProUGUI expoText;
    [SerializeField] private GameObject yesObj;
    [SerializeField] private GameObject noObj;
    [SerializeField] private GameObject backObj;

    //InputSystem
    public InputAction back, decision, move;
    private bool isPointerMove = true;
    //メニュー表示確認Bool
    internal bool isPauseMenu = false;
    private Color color = new Color(255, 69, 0);

    public virtual void InputSet(PlayerInput playerInput, MenuBasic menuBasic = null)
    {
        isPauseMenu = true;
        var input = playerInput;
        back = input.actions["Back"];
        decision = input.actions["Decision"];
        move = input.actions["Move"];

        expoText.text = "セーブデータを削除しますか?";
        ObjSet(true);

        selectMenu = 0;
        OnSelected((int)selectMenu);
    }

    public bool PauseCheck()
    {
        return isPauseMenu;
    }

    public virtual void MenuUpdata()
    {
        StickerChangePointer();

        //選択キーの設定
        if (decision.WasPressedThisFrame())
        {
            SelectMenuProcess();
        }

        if (back.WasPressedThisFrame())
        {
        }
    }

    internal void StickerChangePointer()
    {
        if ((int)selectMenu == 2) return;

        var input = move.ReadValue<Vector2>().x;

        if (input < -0.3f && (int)selectMenu > 0 && isPointerMove)
        {
            isPointerMove = false;
            ChangePointer(-1);
        }
        if (input > 0.3f && (int)selectMenu < 1 && isPointerMove)
        {
            isPointerMove = false;
            ChangePointer(1);
        }
        if (input == 0)
        {
            isPointerMove = true;
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
            case SelectMenu.YES:
                SeveDataDelete();
                break;
            case SelectMenu.NO:
                titleMenu.MenuBack();
                break;
            case SelectMenu.BACK:
                titleMenu.MenuBack();
                break;
        }
    }

    private void SeveDataDelete()
    {
        SeveSystem.Instance.seveDataDelete();
        SceneData.Instance.StageDataReset();
        SceneData.Instance.stock = SceneData.MAX_STOCK;
        expoText.text = "セーブデータを削除しました";
        ObjSet(false);

        OnDeselected((int)selectMenu);
        selectMenu = SelectMenu.BACK;
        OnSelected((int)selectMenu);
    }
    void ObjSet(bool isSet)
    {
        if (isSet)
        {
            yesObj.SetActive(true);
            noObj.SetActive(true);
            backObj.SetActive(false);
        }
        else
        {
            yesObj.SetActive(false);
            noObj.SetActive(false);
            backObj.SetActive(true);
        }
    }

    public virtual MenuSystem Back()
    {
        isPauseMenu = false;
        this.gameObject.SetActive(false);

        return backMenu;
    }

    internal void OnSelected(int objNum)
    {
        if(selectMenu == SelectMenu.BACK)
        {
            target.transform.position = new Vector2(menuObj[objNum].transform.position.x - 280, target.transform.position.y);
        }
        else
        {
            target.transform.position = new Vector2(menuObj[objNum].transform.position.x - 180, target.transform.position.y);
        }
        menuObj[objNum].color = color;    //UIの色変更
    }
    internal void OnDeselected(int objNum)
    {
        menuObj[objNum].color = Color.white; //色を戻す
    }
}
