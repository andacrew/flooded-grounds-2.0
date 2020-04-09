using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepParticles : MonoBehaviour
{
    //Referencing Step particles
    public GameObject grassStep;
    public GameObject dirtStep;
    public GameObject waterStep;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Dirt")
        {
            Debug.Log("Hit dirt");
            GameObject tempDirt = (GameObject)Instantiate(dirtStep);
            tempDirt.transform.position = transform.position;
            Destroy(tempDirt, 1f);
        }
        if (other.gameObject.tag == "Water")
        {
            Debug.Log("Hit water");
            GameObject tempDirt = (GameObject)Instantiate(waterStep);
            tempDirt.transform.position = transform.position;
            Destroy(tempDirt, 1f);
        }
        if (other.gameObject.tag == "Grass")
        {
            Debug.Log("Hit grass");
            GameObject tempDirt = (GameObject)Instantiate(grassStep);
            tempDirt.transform.position = transform.position;
            Destroy(tempDirt, 1f);
        }

    }
}
