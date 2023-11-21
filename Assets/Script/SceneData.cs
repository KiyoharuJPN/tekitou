
public class SceneData
{
    public readonly static SceneData Instance = new();

    //シーンの名前記録
    public string referer = string.Empty;

    //プレイヤー残機
    public int stock = 2;
    public static int MAX_STOCK = 2;

    //復活時確認用Bool
    public bool revival = false;

    //中間地点起動状態
    public bool wayPoint_1 = false;
    public bool wayPoint_2 = false;

    public void DataReset()
    {
        stock = MAX_STOCK;
        wayPoint_1 = false;
        wayPoint_2 = false;
    }

    //プレイ時間
    public float playTime;
}
