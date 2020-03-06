using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyRootMotion : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo stateInfo;

    void Awake()
    {
        anim = this.gameObject.GetComponent<Animator> ();
    }

    void OnAnimatorMove()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo (0);
        if (stateInfo.fullPathHash == Animator.StringToHash("Base.Grounded.Running"))
        {
            anim.ApplyBuiltinRootMotion ();
        }
    }
}