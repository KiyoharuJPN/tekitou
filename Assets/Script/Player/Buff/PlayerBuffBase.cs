using UnityEngine;

//�o�t�t�^�Ɋւ���ϐ��u����
namespace PBF
{
    public static class PlayerBuffBase
    {
        //�o�t�̎��
        public enum BuffType
        {
            ExGage,//�Q�[�W�ǉ�
            SpeedUp,//�X�s�[�h�A�b�v
            Slashing,//�a���g�ǉ�
            Invincible//���G��
        }

        //-------------------------------------------
        //�e�o�t�Ɋւ���ϐ�
        //-------------------------------------------
        //�K�E�Z�Q�[�W�����o�t�p�����[�^
        [System.Serializable]
        public struct ExAttackBuff
        {
            [SerializeField, Header("�����㏸�l")]
            internal int setBuffNum;
            [SerializeField, Header("�l���񐔂ɂ�錸����")]
            internal int setBuffDown;
            //�X�s�[�h�A�b�v�o�t�l����
            internal int getBuffCount;
        };

        //�ړ����x�����o�t�p�����[�^
        [System.Serializable]
        public struct SpeedBuff
        {
            [SerializeField, Header("�����㏸�l")]
            internal float setBuffNum;
            [SerializeField, Header("�l���񐔂ɂ�錸����")]
            internal float setBuffDown;
            [SerializeField, Header("�㏸�l���")]
            internal float maxBuffNum;
            [SerializeField, Header("�U�����x�㏸��")]
            internal float attackSpeedNum;
            //�X�s�[�h�A�b�v�o�t�l����
            internal int getBuffCount;
        };

        //�a���ǉ��o�t�p�����[�^
        [System.Serializable]
        public struct SlashingBuff
        {
            [SerializeField, Header("�����l�����ʎ���")]
            internal float firstSetTime;
            [SerializeField, Header("�o�t���l������")]
            internal float buffSetTime;
            [SerializeField, Header("�l���񐔂ɂ�錸������")]
            internal float buffTimeDown;
            [SerializeField, Header("�o�t�ő厞��")]
            internal float maxBuffTime;
            [SerializeField, Header("�X�s�[�h")]
            internal float slashingSpeed;
            [SerializeField, Header("�a��Prefab")]
            internal GameObject slashingObj;
            //�X�s�[�h�A�b�v�o�t�l����
            internal int getBuffCount;
        };

        //���G���o�t�p�����[�^
        [System.Serializable]
        public struct InvincibleBuff
        {
            [SerializeField, Header("�����l�����ʎ���")]
            internal float firstSetTime;
            [SerializeField, Header("�o�t���l������")]
            internal float buffSetTime;
            [SerializeField, Header("�l���񐔂ɂ�錸������")]
            internal float buffTimeDown;
            [SerializeField, Header("�o�t�ő厞��")]
            internal float maxBuffTime;
            //�X�s�[�h�A�b�v�o�t�l����
            internal int getBuffCount;
        };
        //----------------------------------------------

        
    }
}