using System;
using UnityEngine;

//アニメーション時間取得
public class GetAnimationClipTime
{
    public enum ClipType
    {
        //Heroアニメ
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

        return 0;
    }
}
