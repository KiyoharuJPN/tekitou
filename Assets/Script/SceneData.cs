
public class SceneData
{
    public readonly static SceneData Instance = new();

    //�V�[���̖��O�L�^
    public string referer = string.Empty;

    //�v���C���[�c�@
    public int stock = 2;
    public static int MAX_STOCK = 2;

    //�������m�F�pBool
    public bool revival = false;

    //���Ԓn�_�N�����
    public bool wayPoint = false;

    public void DataReset()
    {
        stock = MAX_STOCK;
        wayPoint = false;
    }
}
