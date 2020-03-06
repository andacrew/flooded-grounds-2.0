using Assets.All_Scripts.Network.Request;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BogLordAttack : MonoBehaviour
{
    public float AttackDamage;

    public ParticleSystem humanBlood;

    private Animator animIzzy;
    private Animator animWinston;
    private Animator animMax;

    // Start is called before the first frame update
    void Start()
    {
        animIzzy = GameObject.FindGameObjectWithTag("Izzy").GetComponent<Animator>();
        animWinston = GameObject.FindGameObjectWithTag("Winston").GetComponent<Animator>();
        animMax = GameObject.FindGameObjectWithTag("Max").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        List<Vector3> particlePos = new List<Vector3>();
        List<Vector3> particleAngle = new List<Vector3>();

        var rot = Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal);
        Destroy(Instantiate(humanBlood.gameObject, other.contacts[0].point, rot), 2f);

        particlePos.Add(other.contacts[0].point);
        particleAngle.Add(rot.eulerAngles);

        RequestHit requestHit = new RequestHit();
        requestHit.setData(other.gameObject.tag, (int)AttackDamage, 1, particlePos, particleAngle);
        requestHit.send();
        Main.GetConnectionManager().send(requestHit);
    }
}
