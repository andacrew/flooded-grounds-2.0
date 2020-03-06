using UnityEngine;
using System.Collections.Generic;


class PickupSpawnPoint : MonoBehaviour
{
    [Header("RESOURCES")]
    public static int maxAmmoMed = 6;
    public static int maxAmmo = 4;
    public static int maxMedpacks = 4;
    public static int maxAk = 5;
    public static int maxShotgun = 5;
    public static int maxPistol = 5;
    

    private static int medpackCount;
    private static int ammoCount;
    private static int pistolCount;
    private static int akCount;
    private static int shotgunCount;

    private List<GameObject> items;
    private static System.Random rnd;
    private int pickup;
    private GameObject item;
    
    void Awake()
    {
        items = new List<GameObject>();
        items.Add(transform.Find("med").gameObject);
        items.Add(transform.Find("ammo").gameObject);
        items.Add(transform.Find("Pistol").gameObject);
        items.Add(transform.Find("AK-47").gameObject);
        items.Add(transform.Find("Shotgun").gameObject);
        rnd = new System.Random();           
    }

    void Start()
    {
        pickup = rnd.Next(5);
        switch (pickup)
        {
            case 0:
                if (medpackCount < maxMedpacks && ammoCount + medpackCount < maxAmmoMed)
                {
                    item = transform.Find("med").gameObject;
                    medpackCount++;
                }
                break;
            case 1:
                if (ammoCount < maxAmmo && ammoCount + medpackCount < maxAmmoMed)
                { 
                    item = transform.Find("ammo").gameObject;
                    ammoCount++;
                }
                break;
            case 2:
                if (akCount < maxAk)
                {
                    item = transform.Find("AK-47").gameObject;
                    akCount++;
                }
                break;
            case 3:
                if (pistolCount < maxPistol)
                {
                    item = transform.Find("Pistol").gameObject;
                    pistolCount++;
                }
                break;
            case 4:
                if (shotgunCount < maxShotgun)
                {
                    item = transform.Find("Shotgun").gameObject;
                    shotgunCount++;
                }
                break;
        }
         
        // remove item we want from list
        if (item != null)
            items.Remove(item);

        // disable all the other items in the list, we don't want them 
        for (int i = 0; i < items.Count; i++)
            items[i].SetActive(false);
    }
}