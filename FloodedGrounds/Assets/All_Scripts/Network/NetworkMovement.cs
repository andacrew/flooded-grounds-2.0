using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMovement : MonoBehaviour
{
    //The the old values from one update ago
    private Vector3 oldPosition;
    private Vector3 oldLookAngle;
    private float oldYRotation;

    //The target values of the parameters that will be interpolated to
    private Vector3 targetPosition;
    private Vector3 targetLookAngle;
    private float targetYRotation;

    //A number between 0 and 1 representing how far between the two values to interpolate
    private float interpolationProgress;
    //How long the interpolation should last for
    private float interpolationLength;
    //The time when the last update was received
    private float startTime;

    Animator anim;

    //Set dummy variables for the target parameters
    private void Start()
    {
        anim = GetComponent<Animator>();

        targetPosition = transform.position;
        targetLookAngle = new Vector3();
        targetYRotation = transform.rotation.eulerAngles.y;
        //The interpolation should last approximately until the next update arrives
        interpolationLength = 1f / Constants.updatesPerSecond;

        //Set the start time
        startTime = Time.time;
    }

    //Update the paramters
    public void updateParams(Vector3 newPosition, Vector3 newLookAngle, float newYRotation)
    {
        //Set the old parameters to the current position
        oldPosition = transform.position;
        oldLookAngle = new Vector3();
        oldYRotation = transform.rotation.eulerAngles.y;

        //Set the new target parameters
        targetPosition = newPosition;
        targetLookAngle = newLookAngle;
        targetYRotation = newYRotation;

        //Reset the interpolation progress and start time
        interpolationProgress = 0f;
        startTime = Time.time;
    }
    
    //Interpolate all three parameters
    void Update()
    {
        //Calculate how far in between the two values to interpolate
        interpolationProgress = (Time.time - startTime) / interpolationLength;

        //Interpolate the parameters
        transform.position = Vector3.Lerp(oldPosition, targetPosition, interpolationProgress);
        Vector3.LerpUnclamped(oldLookAngle, targetLookAngle, interpolationProgress);
        transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, Mathf.LerpAngle(oldYRotation, targetYRotation, interpolationProgress), transform.rotation.eulerAngles.z);
    }

    // Ends the animation states
    void EndShooting()
    {
        anim.SetBool("isShooting", false);
    }

    void EndJump()
    {
        anim.SetBool("isJumping", false);
    }

    void EndShout()
    {
        anim.SetBool("isShouting", false);
    }

    void EndAttack()
    {
        anim.SetBool("isAttacking", false);
    }
}