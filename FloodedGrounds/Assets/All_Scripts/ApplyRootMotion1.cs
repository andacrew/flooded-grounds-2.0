using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyRootMotion1 : StateMachineBehaviour
{
    void OnAnimatorMove()
    {
        Animator anim = GetComponent<Animator>();
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        anim.ApplyBuiltinRootMotion();
    }

    private T GetComponent<T>()
    {
        throw new NotImplementedException();
    }
}
