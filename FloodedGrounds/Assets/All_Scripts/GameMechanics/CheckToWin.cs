using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckToWin : MonoBehaviour
{
    public ManagementHUD hud;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Max" || other.gameObject.tag == "Winston" || other.gameObject.tag == "Izzy")
        {
            if (hud.hasKeys && hud.hasGas && hud.hasSteeringWheel)
            {
                Debug.Log("You Win!");
            }
            else
            {
                Debug.Log("You're still missing items before you can escape!");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
