using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public const int maxHealth = 100;
    public int currentHealth = maxHealth;
    //public RectTransform healthbar;
    public RectTransform healthHUD;
    Animator anim;

    public SkinnedMeshRenderer rend;
    public Canvas canvas;

    public void takeDamage(int amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            anim.SetBool("isDead", true);
            Debug.Log("Dead");
            StartCoroutine(Respawn());
        }

        healthHUD.sizeDelta = new Vector2((float)(currentHealth * 1.5), healthHUD.sizeDelta.y);
        //healthbar.sizeDelta = new Vector2(currentHealth, healthbar.sizeDelta.y);
    }
    
    IEnumerator Respawn()
    {
        canvas.enabled = false;
        yield return new WaitForSeconds(2);
        rend.enabled = false;
        yield return new WaitForSeconds(3);
        this.transform.position = new Vector3(8, 0, 8);
        anim.SetBool("isDead", false);
        anim.Play("idle");
        currentHealth = 100;
        healthHUD.sizeDelta = new Vector2((float)(currentHealth * 1.5), healthHUD.sizeDelta.y);
        //healthbar.sizeDelta = new Vector2(currentHealth, healthbar.sizeDelta.y);
        canvas.enabled = true;
        rend.enabled = true;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
    }
}
