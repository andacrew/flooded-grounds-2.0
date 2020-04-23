using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactermovement : MonoBehaviour
{

    CharacterController cc;
    public float moveSpeed = 4f;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        cc = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        Movement();
        
    }

    void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("isRunning", true);
            moveSpeed = 8f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            anim.SetBool("isRunning", false);
            moveSpeed = 4f;
        }
    }

    void Movement()
    {
        //Left/Right/Up/Down Input
        float hdir = Input.GetAxis("Horizontal");
        float vdir = Input.GetAxis("Vertical");

        //Moving the character
        Vector3 direction = new Vector3(hdir, 0, vdir);
        Vector3 velocity = direction * moveSpeed * Time.deltaTime;
        cc.Move(velocity);

        Debug.Log("hdir: " + hdir + "\n" + "vdir: " + vdir);

        MoveAnimations(hdir,vdir);

    }

    void MoveAnimations(float hdir, float vdir)
    {
        anim.SetFloat("horizontal", hdir);
        anim.SetFloat("vertical", vdir); 
    }
}
