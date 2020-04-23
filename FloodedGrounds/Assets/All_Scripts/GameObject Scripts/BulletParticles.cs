using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticles : MonoBehaviour
{
    //Referencing BulletHit particles
    public GameObject Hit;
    public GameObject playerBlood;
    public GameObject monsterBlood;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject tempHit = (GameObject)Instantiate(Hit);
        tempHit.transform.position = transform.position;
        Destroy(tempHit, 1f);

        if (other.gameObject.tag == "Monster")
        {
            Debug.Log("Hit Monster");
            GameObject tempMonsterBlood = (GameObject)Instantiate(monsterBlood);
            tempMonsterBlood.transform.position = transform.position;
            Destroy(tempMonsterBlood, 1f);
        }
        if (other.gameObject.tag == "Human")
        {
            Debug.Log("Hit Player");
            GameObject tempPlayerBlood = (GameObject)Instantiate(playerBlood);
            tempPlayerBlood.transform.position = transform.position;
            Destroy(tempPlayerBlood, 1f);
        }
    }
}
