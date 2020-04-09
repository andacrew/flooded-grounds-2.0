using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRaycasting : MonoBehaviour
{
    public ManagementHUD counter;
    public Animator medAnim;
    public Animator ammoAnim;

    public float maxDistance = 3;
    public float sphereRadius = 0.5f;

    public float currentHitDistance;

    public GameObject player;
    private MaxMovement list;


    // Start is called before the first frame update
    void Start()
    {
        list = player.GetComponent<MaxMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * maxDistance, Color.magenta);
        //Gizmos.DrawWireSphere(this.transform.position + this.transform.forward * maxDistance, sphereRadius);

        RaycastHit hit;

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Medpack")
        {
            medAnim = hit.collider.gameObject.GetComponent<Animator>();
            medAnim.enabled = true;

            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                Destroy(hit.collider.gameObject);
                counter.MedCounter(1);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Ammo")
        {
            ammoAnim = hit.collider.gameObject.GetComponent<Animator>();
            ammoAnim.enabled = true;

            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                Destroy(hit.collider.gameObject);
                counter.AmmoPickup();
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Pistol")
        {
            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                list.heldGuns.Add(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                //Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Shotgun")
        {
            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                list.heldGuns.Add(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                //Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "AK-47")
        {
            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                list.heldGuns.Add(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                //Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "CarKey")
        {
            //Debug.Log("Raycast hit keys");
            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                counter.hasKeys = true;
                //Debug.Log("Interact keys");
                //list.heldGuns.Add(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                //Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Gas")
        {
            //Debug.Log("Raycast hit gas");
            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                counter.hasGas = true;
                //Debug.Log("Interact gas");
                //list.heldGuns.Add(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                //Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

        if (Physics.SphereCast(this.transform.position, sphereRadius, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "SteeringWheel")
        {
            //Debug.Log("Raycast hit steering wheel");
            currentHitDistance = hit.distance;

            counter.InteractHint();

            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestPickup pickup = new RequestPickup();
                pickup.setPickupName(hit.collider.gameObject.name);
                pickup.send();
                Main.GetConnectionManager().send(pickup);

                counter.hasSteeringWheel = true;
                //Debug.Log("Interact steering wheel");
                //list.heldGuns.Add(hit.collider.gameObject);
                hit.collider.gameObject.SetActive(false);
                //Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            currentHitDistance = maxDistance;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * currentHitDistance);
        Gizmos.DrawWireSphere(this.transform.position + this.transform.forward * currentHitDistance, sphereRadius);
    }

}

//if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Medpack")
//{
//    medAnim = hit.collider.gameObject.GetComponent<Animator>();
//    medAnim.enabled = true;

//    if (Input.GetKeyDown(KeyCode.E))
//    {
//        Destroy(hit.collider.gameObject);
//        //medPack = false;
//        counter.MedCounter(1);
//    }
//}
//if (Physics.Raycast(this.transform.position, this.transform.forward, out hit, maxDistance) && hit.collider.gameObject.tag == "Ammo")
//{
//    ammoAnim = hit.collider.gameObject.GetComponent<Animator>();
//    ammoAnim.enabled = true;

//    if (Input.GetKeyDown(KeyCode.E))
//    {
//        Destroy(hit.collider.gameObject);
//        counter.AmmoPickup(120);
//    }
//}
