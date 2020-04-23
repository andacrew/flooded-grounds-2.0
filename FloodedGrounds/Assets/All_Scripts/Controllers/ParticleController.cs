using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    //For now we'll have this on player1
    [SerializeField]
    private GameObject player1;
    public GameObject Feet;

    //Particle Asset References
    public GameObject grassStep;
    public GameObject waterStep;
    public GameObject dirtStep;
    public GameObject BulletHit;
    public GameObject playerHurt;
    public GameObject MonsterHurt;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
