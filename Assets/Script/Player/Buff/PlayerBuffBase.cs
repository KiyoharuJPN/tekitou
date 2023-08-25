using UnityEngine;

//バフ付与に関する変数置き場
namespace PBF
{
    public static class PlayerBuffBase
    {
        //バフの種類
        public enum BuffType
        {
            ExGage,//ゲージ追加
            SpeedUp,//スピードアップ
            Slashing,//斬撃波追加
            Invincible//無敵化
        }

        //-------------------------------------------
        //各バフに関する変数
        //-------------------------------------------
        //必殺技ゲージ増加バフパラメータ
        [System.Serializable]
        public struct ExAttackBuff
        {
            [SerializeField, Header("初期上昇値")]
            internal int setBuffNum;
            [SerializeField, Header("獲得回数による減少量")]
            internal int setBuffDown;
            //スピードアップバフ獲得回数
            internal int getBuffCount;
        };

        //移動速度増加バフパラメータ
        [System.Serializable]
        public struct SpeedBuff
        {
            [SerializeField, Header("初期上昇値")]
            internal float setBuffNum;
            [SerializeField, Header("獲得回数による減少量")]
            internal float setBuffDown;
            [SerializeField, Header("上昇値上限")]
            internal float maxBuffNum;
            [SerializeField, Header("攻撃速度上昇量")]
            internal float attackSpeedNum;
            //スピードアップバフ獲得回数
            internal int getBuffCount;
        };

        //斬撃追加バフパラメータ
        [System.Serializable]
        public struct SlashingBuff
        {
            [SerializeField, Header("初期獲得効果時間")]
            internal float firstSetTime;
            [SerializeField, Header("バフ中獲得時間")]
            internal float buffSetTime;
            [SerializeField, Header("獲得回数による減少時間")]
            internal float buffTimeDown;
            [SerializeField, Header("バフ最大時間")]
            internal float maxBuffTime;
            [SerializeField, Header("スピード")]
            internal float slashingSpeed;
            [SerializeField, Header("斬撃Prefab")]
            internal GameObject slashingObj;
            //スピードアップバフ獲得回数
            internal int getBuffCount;
        };

        //無敵化バフパラメータ
        [System.Serializable]
        public struct InvincibleBuff
        {
            [SerializeField, Header("初期獲得効果時間")]
            internal float firstSetTime;
            [SerializeField, Header("バフ中獲得時間")]
            internal float buffSetTime;
            [SerializeField, Header("獲得回数による減少時間")]
            internal float buffTimeDown;
            [SerializeField, Header("バフ最大時間")]
            internal float maxBuffTime;
            //スピードアップバフ獲得回数
            internal int getBuffCount;
        };
        //----------------------------------------------

        
    }
}