using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeItem : MonoBehaviour {

    public Transform objectParent;
    public GameObject itemObject;

    public Sprite icon;
    public Image iconObject;

    public string itemText;
    public Text textObject;

    public void SendItem()
    {
        GameObject go = Instantiate(itemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        go.transform.parent = objectParent;

    }

    public void SetItem()
    {
        iconObject.sprite = icon;
        textObject.text = itemText;
    }

    void Update ()
    {
        if (Input.GetKeyDown("f"))
        {
            SetItem();
            SendItem();
        }
    }
}
