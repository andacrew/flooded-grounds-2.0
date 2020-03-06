using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillNotification : MonoBehaviour {

    public Transform objectParent;
    public GameObject notificationObject;

    public string enemyNickname;
    public string usedWeapon;
    public Text enemyNickText;
    public Text usedWeaponText;
    public bool isTop;

    public void SendNotification()
    {
        GameObject go = Instantiate(notificationObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        go.transform.parent = objectParent;

    }

    public void SetNotification()
    {
        if (isTop == false)
        {
            enemyNickText.text = enemyNickname;
            usedWeaponText.text = usedWeapon;
        }

    }

    void Update ()
    {
        if (Input.GetKeyDown("k"))
        {
            SetNotification();
            SendNotification();
        }
    }
}
