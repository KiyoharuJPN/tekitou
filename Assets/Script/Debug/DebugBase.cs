using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�f�o�b�O�X�N���v�gBase
public class DebugBase : MonoBehaviour
{
    

    //�v���C���[����
    [SerializeField]
    internal GameObject p_obj;
    internal PlayerController p_Controller;
    internal Player_Walk p_Walk;
    internal Player_Jump p_Jump;

    private void Start()
    {

        if (p_obj != null)
        {
            p_Controller = p_obj.GetComponent<PlayerController>();
            p_Walk = p_obj.GetComponent<Player_Walk>();
            p_Jump = p_obj.GetComponent<Player_Jump>();
        }
    }
}
