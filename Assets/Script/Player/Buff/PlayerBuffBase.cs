using UnityEngine;

//�o�t�t�^�Ɋւ���ϐ��u����
namespace PBF
{
    public static class PlayerBuffBase
    {
        //-------------------------------------------
        //�e�o�t�Ɋւ���ϐ�
        //-------------------------------------------
        //�K�E�Z�Q�[�W�����o�t�p�����[�^
        [System.Serializable]
        public struct exAttackBuff
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
        public struct speedBuff
        {
            [SerializeField, Header("�����㏸�l")]
            internal float setBuffNum;
            [SerializeField, Header("�l���񐔂ɂ�錸����")]
            internal float setBuffDown;
            //�X�s�[�h�A�b�v�o�t�l����
            internal int getBuffCount;
        };

        //�a���ǉ��o�t�p�����[�^
        [System.Serializable]
        public struct slashingBuff
        {
            [SerializeField, Header("�����l�����ʎ���")]
            internal float firstSetTime;
            [SerializeField, Header("�o�t���l������")]
            internal float buffSetTime;
            [SerializeField, Header("�l���񐔂ɂ�錸������")]
            internal float buffTimeDown;
            [SerializeField, Header("�o�t�ő厞��")]
            internal float maxBuffTime;
            [SerializeField, Header("�o�t�ő厞��")]
            internal GameObject slashingObj;
            //�X�s�[�h�A�b�v�o�t�l����
            internal int getBuffCount;
        };

        //���G���o�t�p�����[�^
        [System.Serializable]
        public struct invincibleBuff
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