using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footfall : MonoBehaviour
{
    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.ParameterInstance myParameter;
    Animator anim;

    public GameObject playerObj;

    public bool isIndoors;
    public bool isMoving;

    int frames = 0;
    int framesDiv = 0;

    void Awake()
    {
        //var target = GameObject.Find(playerObj);
        var target = playerObj;
        emitter = target.GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter("Surface_index", 1.1f);
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get current animation status
        isMoving = anim.GetBool("isWalking");

        // Updates to Play() more/less if running/walking
        if (anim.GetBool("isRunning") == true)
            framesDiv = 20;
        else
            framesDiv = 30;

        if (isIndoors)
            // Walking on wooden floors sfx adaption
            emitter.SetParameter("Surface_index", 2.1f);
        else
            // Walking on dirt sfx adaption
            emitter.SetParameter("Surface_index", 1.1f);

        if (frames % framesDiv == 0)
        {
            if (isMoving)
                GetComponent<FMODUnity.StudioEventEmitter>().Play();

            else
                GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        }

        frames += 1;
        if (frames == 60)
            frames = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        isIndoors |= other.gameObject.tag == "Indoors";
    }

    void OnTriggerExit(Collider other)
    {
        isIndoors = false;
    }
}