using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExAttack_LastEffect : MonoBehaviour
{
    private Animation anim;

    public AnimationClip lastEffect;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animation>();
        anim.clip = lastEffect;
        anim.Play();
    }
}
