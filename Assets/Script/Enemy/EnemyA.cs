using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class EnemyA : Enemy
{
    Animator animator;  //�G�̃A�j���֐�
    //bool IsBlowing;     //��΂�����Ԃ̃`�F�b�N
    override protected void Start()
    {
        //�����p�A�j���[�^�[�̑��
        animator = GetComponent<Animator>();
        ///�G��script�Ɋ�Â�
        base.Start();
    }
    override protected void Update()
    {
        //�A�j���[�^�[�̐ݒ�
        animator.SetBool("IsBlowing",IsBlowing);
        //��Ԃ̕ύX
        if (isDestroy) IsBlowing = true;
        if (!isDestroy) IsBlowing = false;
        //�G��script�Ɋ�Â�
        base.Update();
    }
}
