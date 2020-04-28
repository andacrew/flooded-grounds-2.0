using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    public string loseScene = "LoseScene";
    public string loadScene;
    //public RectTransform healthbar;
    public RectTransform healthHUD;
    Animator anim;


    public SkinnedMeshRenderer rend;
    public Canvas canvas;
    public UnityAction onDie;

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            //loadScene = SceneManagement.GetActiveScene().buildIndex + 2;
            loadScene = loseScene;
            SceneManagement.LoadScene(loadScene);
            Debug.Log("You lose!");
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
    }
}