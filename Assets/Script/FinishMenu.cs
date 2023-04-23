using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FinishMenu : MonoBehaviour
{
    [Tooltip("�ڕW�̃V�[���l�[���������Ă�������")]
    public string SceneName;


    public GameObject[] Finishobj;

    //�|�C���^�[
    int pointer = 0, pointerpreb = -1;

    private void Awake()
    {
        OnSelected(Finishobj[0]);
    }
    // Update is called once per frame
    void Update()
    {
        //�����L�[�̐ݒ�
        if(Input.GetKeyDown(KeyCode.W))
        {
            pointer--;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            pointer++;
        }


        //�|�C���^�[���ς�������̐ݒ�
        if(pointer != pointerpreb)
        {
            if (pointer < 0) pointer = Finishobj.Length - 1;
            if (pointer > Finishobj.Length - 1) pointer = 0;

            OnSelected(Finishobj[pointer]);
            Debug.Log("pointer"+pointer + '\n' + "pointerpreb"+pointerpreb);
            if (pointer != pointerpreb && pointerpreb != -1) OnDeselected(Finishobj[pointerpreb]);

            //�|�C���^�[��������
            pointerpreb = pointer;
        }

        //�I���L�[�̎w��
        if (Input.GetKeyUp(KeyCode.Space))
        {
            switch (pointer)
            {
                case 0:
                    Debug.Log(SystemController.Instance.GetLastScene());
                    SceneManager.LoadScene(SystemController.Instance.GetLastScene());
                    break;
                case 1:
                    if (SceneName != "") SceneManager.LoadScene(SceneName);
                    break;
                default:
                    Debug.Log("�V�������ڂ̒ǉ��̓v���O���}�ɗ���ł��������B");
                    break;
            }
        }
    }





    //��������
    void OnSelected(GameObject obj)
    {
        obj.GetComponent<Image>().color = Color.grey;               //UI�̐F�C��
    }
    void OnDeselected(GameObject obj)
    {
        obj.GetComponent<Image>().color = new Color(255, 255, 255); //�F��߂�
    }
}
