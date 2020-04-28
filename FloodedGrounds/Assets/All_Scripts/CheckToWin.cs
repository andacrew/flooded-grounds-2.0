using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class CheckToWin : MonoBehaviour
{
    public ManagementHUD hud;
    public string winScene = "WinScene";
    public string loseScene = "LoseScene";
    public string loadScene; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Max" || other.gameObject.tag == "Winston" || other.gameObject.tag == "Izzy")
        {
            if (hud.hasKeys && hud.hasGas && hud.hasSteeringWheel)
            {
                //loadScene = SceneManagement.GetActiveScene().buildIndex + 1;
                loadScene = winScene;
                SceneManagement.LoadScene(loadScene);
                Debug.Log("You Win!");
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
