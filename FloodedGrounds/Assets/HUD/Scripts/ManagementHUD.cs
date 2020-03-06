using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagementHUD : MonoBehaviour
{
    public int ammoIn, ammoOut;
    public string gunEquipped;
    public bool hasKeys, hasGas, hasSteeringWheel;

    private Dictionary<string, Tuple<int, int>> myGuns = new Dictionary<string, Tuple<int, int>>();
    private Dictionary<string, int> maxMag = new Dictionary<string, int>();

    public int _medPack = 1;

    public Text ammoDisplay;
    public Text medpack;

    public Text interactHint;
    private TextFadeOut hintText;

    void Start()
    {
        myGuns.Add("Pistol", new Tuple<int, int>(12, 36));
        myGuns.Add("AK-47", new Tuple<int, int>(30, 90));
        myGuns.Add("Shotgun", new Tuple<int, int>(8, 24));
        myGuns.Add("Knife", new Tuple<int, int>(0, 0));

        maxMag.Add("Pistol", 12);
        maxMag.Add("AK-47", 30);
        maxMag.Add("Shotgun", 8);

        ammoIn = 0;
        ammoOut = 0;

        hintText = interactHint.GetComponent<TextFadeOut>();
        hasKeys = false;
        hasGas = false;
        hasSteeringWheel = false;
    }

    // Updating HUD depending on which weapon is equipped 
    public void UpdateHUDAmmo()
    {
        if (gunEquipped == "Knife")
        {
            ammoIn = 0;
            ammoOut = 0;
            ammoDisplay.text = "\u221E <size=8>/ \u221E </size>";
        }
        else
        {
            ammoIn = (myGuns[gunEquipped]).Item1;
            ammoOut = (myGuns[gunEquipped]).Item2;

            ammoDisplay.text = ammoIn.ToString() + " <size=8>/ " + ammoOut.ToString() + "</size>";
        }
    }

    public void UpdateAmmo()
    {
        myGuns[gunEquipped] = new Tuple<int, int>(ammoIn, ammoOut);
    }

    // General ammo counter for all guns
    public void AmmoCounter(int amount)
    {
        ammoIn -= amount;

        if (ammoIn <= 0 && ammoOut <= 0)
        {
            ammoIn = 0;
            ammoOut = 0;
            Debug.Log("Out of ammo! Find more ammo!");
        }
        else if (ammoIn <= 0 && ammoOut > 0)
        {
            ammoIn = 0;
            Debug.Log("Reload!");
        }

        UpdateAmmo();
        ammoDisplay.text = ammoIn.ToString() + " <size=8>/ " + ammoOut.ToString() + "</size>";
    }

    public void AmmoReload()
    {
        if (maxMag[gunEquipped] - ammoIn >= ammoOut)
        {
            ammoIn += ammoOut;
            ammoOut = 0;
        }
        else
        {
            ammoOut -= (maxMag[gunEquipped] - ammoIn);
            ammoIn = maxMag[gunEquipped];
        }

        UpdateAmmo();
        ammoDisplay.text = ammoIn.ToString() + " <size=8>/ " + ammoOut.ToString() + "</size>";
    }

    public void MedCounter(int add)
    {
        _medPack += add;

        medpack.text = _medPack.ToString();
    }

    public void AmmoPickup()
    {
        myGuns["Pistol"] = new Tuple<int, int>(myGuns["Pistol"].Item1, myGuns["Pistol"].Item2 + 24);
        myGuns["AK-47"] = new Tuple<int, int>(myGuns["AK-47"].Item1, myGuns["AK-47"].Item2 + 60);
        myGuns["Shotgun"] = new Tuple<int, int>(myGuns["Shotgun"].Item1, myGuns["Shotgun"].Item2 + 16);

        UpdateHUDAmmo();
    }

    public void InteractHint()
    {
        //Display hint to interact with objects 
        interactHint.enabled = true;
        hintText.FadeOut();
    }


    // Update is called once per frame
    void Update()
    {

    }
}