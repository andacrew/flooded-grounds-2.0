using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepEventEmitter : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string FootstepEventString = "";

    void OnCollisionEnter()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FootstepEventString, GetComponent<Transform>().position);
    }

}
