using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement2 : MonoBehaviour
{
    public float speed = 1.0f;
    public float gravity = -9.8f;
    public bool gravityEnabled = true;
    Vector3 moveDir = Vector3.zero;
    public float maxHP = 150f;
    float recoveringHP = 0f;
    private bool died = false;

    //Jump variables
    public float speed2 = 3.0f;
    public float jumpSpeed = 4.0f;
    public float gravity2 = 10.0f;
    private Vector3 moveDirection = Vector3.zero;

    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject youLoseScreen;

    private float playerHealth;
    public GameObject HPCanvas;

    Animator anim;
    private Rigidbody rb;
    CharacterController controller;

    public GameObject shout;

    public Transform spine;
    public Transform leftLeg;
    public Transform rightLeg;

    public float sensitivity = 30.0f;
    public float WaterHeight = 15.5f;
    public GameObject cam;
    public float rotX, rotY;
    public bool webGLRightClickRotation = true;

    // Start is called before the first frame update
    void Start()
    {
        //Leave gravityEnabled to false for jumping to work
        gravityEnabled = false;

        rb = this.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        
        //LockCursor ();
        if (Application.isEditor)
        {
            webGLRightClickRotation = false;
            sensitivity = sensitivity * 1.5f;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //CheckCamera();
        CheckForWaterHeight();
        Jump();
        Movement();
        GetInput();
    }

    private void FixedUpdate()
    {
       Invoke("CheckIfDead", 1f);
       //CheckIfDead();
    }

    void CameraRotation(GameObject cam, float rotX, float rotY)
    {
        transform.Rotate(0, rotX * Time.deltaTime, 0);
        cam.transform.Rotate(-rotY * Time.deltaTime, 0, 0);
    }

    void CheckCamera()
    {
        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;
        Debug.Log("rotY: " + rotY);
        if (webGLRightClickRotation)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                CameraRotation(cam, rotX, rotY);
            }
        }

        else if (!webGLRightClickRotation)
        {
            CameraRotation(cam, rotX, rotY);
        }

        moveDir = transform.rotation * moveDir;
    }

    void GetInput()
    {
        // Attack
        if (Input.GetMouseButton(0)) // left mouse button is pressed
        {
            anim.SetBool("isAttacking", true);
        }

        // Jump
        if (Input.GetKey(KeyCode.Space))
        {
            anim.SetBool("isJumping", true);
        }

        // Shout
        if (Input.GetKey(KeyCode.X))
        {
            anim.SetBool("isShouting", true);
        }
    }

    void Shouting()
    {
        GameObject tempShout = (GameObject)Instantiate(shout);
        tempShout.transform.position = transform.position;
        Destroy(tempShout, 5.0f);
    }

    void Movement()
    {
        float vdir = Input.GetAxis("Vertical");
        float hdir = Input.GetAxis("Horizontal");

        // Holding shift Running
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 3.0f;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 1.0f;
        }

        // Moving forward-left
        if (hdir < 0 && vdir > 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isForward", true);
            anim.SetBool("isLeft", true);
            anim.SetFloat("Speed", speed);
            spine.transform.Rotate(45, 0, 0);
            leftLeg.transform.Rotate(-15, 0, 0);
            rightLeg.transform.Rotate(-15, 0, 0);
        }

        // Moving backwards-left
        else if (hdir < 0 && vdir < 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isBackward", true);
            anim.SetBool("isLeft", true);
            anim.SetFloat("Speed", speed);
            spine.transform.Rotate(45, 0, 0);
            leftLeg.transform.Rotate(-15, 0, 0);
            rightLeg.transform.Rotate(-15, 0, 0);
        }

        // Moving forward-right
        else if (hdir > 0 && vdir > 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isForward", true);
            anim.SetBool("isRight", true);
            anim.SetFloat("Speed", speed);
            spine.transform.Rotate(-45, 0, 0);
            leftLeg.transform.Rotate(15, 0, 0);
            rightLeg.transform.Rotate(15, 0, 0);
        }

        // Moving backwards-right
        else if (hdir > 0 && vdir < 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isBackward", true);
            anim.SetBool("isRight", true);
            anim.SetFloat("Speed", speed);
            spine.transform.Rotate(-45, 0, 0);
            leftLeg.transform.Rotate(15, 0, 0);
            rightLeg.transform.Rotate(15, 0, 0);
        }

        // Moving forward
        else if (vdir > 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isForward", true);
            anim.SetFloat("Speed", speed);
        }

        // Moving backwards
        else if (vdir < 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isBackward", true);
            anim.SetFloat("Speed", speed);
        }

        // Moving towards right
        else if (hdir > 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRight", true);
            anim.SetFloat("Speed", speed);
            spine.transform.Rotate(-45, 0, 0);
            leftLeg.transform.Rotate(15, 0, 0);
            rightLeg.transform.Rotate(15, 0, 0);
        }

        // Moving towards left
        else if (hdir < 0)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("isLeft", true);
            anim.SetFloat("Speed", speed);
            spine.transform.Rotate(45, 0, 0);
            leftLeg.transform.Rotate(-15, 0, 0);
            rightLeg.transform.Rotate(-15, 0, 0);
        }

        // Stationary
        else
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isLeft", false);
            anim.SetBool("isRight", false);
            anim.SetBool("isForward", false);
            anim.SetBool("isBackward", false);
            anim.SetFloat("Speed", 0.0f);
        }

        if (!died)
        {
            // Update character's location and gravity
            if (gravityEnabled)
                moveDir = new Vector3(hdir, gravity, vdir);
            else
                moveDir = new Vector3(hdir, 0, vdir);

            moveDir *= speed;
            moveDir = transform.TransformDirection(moveDir);
            controller.Move(moveDir * Time.deltaTime);
        }
    }

    void CheckForWaterHeight()
    {
        if (transform.position.y < WaterHeight)
        {
            gravity = 0f;
        }

        else
        {
            gravity = -9.8f;
        }
    }

    void CheckIfDead()
    {
        playerHealth = HPCanvas.GetComponent<RectTransform>().rect.xMax;

        if (playerHealth <= 0 && died == false)
        {
            
            anim.SetBool("isDead", true);
           // Debug.Log("Is dead");
            died = true;
            youLoseScreen.SetActive(true);
        }
        

        if(died == true)
        {            
            //Debug.Log("i die");
            if (recoveringHP < maxHP)
            {
                HPCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, recoveringHP);
                recoveringHP += 10f * Time.deltaTime;
            }

            else
            {
                youLoseScreen.SetActive(false);
                died = false;
                anim.SetBool("isDead", false);
                recoveringHP = 0f;
            }
        }

    }

    void checkIfHit()
    {
        // If receive raycast shot
        if (!died)
        {
            anim.SetBool("isHit", true);
        }

        else if (died)
        {
            anim.SetBool("isDead", true);
        }

        // If not receiving raycast shot
        anim.SetBool("isHit", false);
    }

    public void TakeDamageFromPlayer(float damage)
    {
        //Store current visual HP bar into "Playerhealth"
        playerHealth = HPCanvas.GetComponent<RectTransform>().rect.width;

        //Set the new visual HP bar's stat the same width minus 50
        HPCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerHealth - damage);
    }

    void StartCollider()
    {
        rightHand.SetActive(true);
        leftHand.SetActive(true);
    }

    void EndCollider()
    {
        rightHand.SetActive(false);
        leftHand.SetActive(false);
    }

    void Jump()
    {
        if (controller.isGrounded)
        {

            moveDirection = new Vector3(0f, 0.0f, 0f);
            moveDirection *= speed2;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity2 * Time.deltaTime;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
    }
}