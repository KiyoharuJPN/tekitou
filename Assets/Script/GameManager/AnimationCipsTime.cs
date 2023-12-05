using System;
using UnityEngine;

public class AnimationCipsTime
{
    public enum ClipType
    {
        //Hero�A�j��
        Hero_anim_1,
        Hero_knockBack,
        ExAttack,
        WarpDoor,
        HeroGool,
        NomalAttack_Stage,
        NomalAttack_Jump,
        Hero_UpAttack_Start,
        Hero_DropAttack_Start,
        Hero_SideAttack_Start,
        Hero_UpAttack_End,
        Hero_DropAttack_End,
        //�Z�C�J�A�j��
        SeikaNomalAttack_Stage,
        SeikaNomalAttack_Jump,
        Seika_UpAttack_Start,
        Seika_DropAttack_Start,
        Seika_SideAttack_Start,
        Seika_UpAttack_End,
        Seika_DropAttack_End,
    }

    static public float GetAnimationTime(Animator animator, ClipType type)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            if (clip.name == type.ToString())
            {
                return clip.length;
            }
        }

        return 0.0f;
    }
}
