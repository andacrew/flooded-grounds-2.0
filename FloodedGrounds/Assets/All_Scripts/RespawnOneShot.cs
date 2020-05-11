using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOneShot : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Gameplay Actions/Footsteps/Player (Robot) Footsteps", transform.position);
    }
}
