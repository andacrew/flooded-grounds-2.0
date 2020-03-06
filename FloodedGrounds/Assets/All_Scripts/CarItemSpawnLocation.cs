using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarItemSpawnLocation : MonoBehaviour
{
    public GameObject carKey, gasCan, steeringWheel;

    private IList<Vector3> carItemSpawnPos = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        // Set possible spawn positions 
        carItemSpawnPos.Add(new Vector3(405, 18, 408));
        carItemSpawnPos.Add(new Vector3(483, 18, 732));
        carItemSpawnPos.Add(new Vector3(558, 18, 649));
        carItemSpawnPos.Add(new Vector3(380, 26, 675));
        carItemSpawnPos.Add(new Vector3(493, 18, 582));
        carItemSpawnPos.Add(new Vector3(648, 18, 513));
        carItemSpawnPos.Add(new Vector3(822, 22, 521));
        carItemSpawnPos.Add(new Vector3(719, 18, 408));
        carItemSpawnPos.Add(new Vector3(557, 20, 175));
        carItemSpawnPos.Add(new Vector3(526, 18, 442));

        // Spawn objects at start of game
        SpawnCarItems();
    }

    void SpawnCarItems()
    {
        int spawn1 = Random.Range(0, carItemSpawnPos.Count);

        Instantiate(carKey, carItemSpawnPos[spawn1], Quaternion.identity);
        carItemSpawnPos.RemoveAt(spawn1);

        int spawn2 = Random.Range(0, carItemSpawnPos.Count);

        Instantiate(gasCan, carItemSpawnPos[spawn2], Quaternion.identity);
        carItemSpawnPos.RemoveAt(spawn2);

        int spawn3 = Random.Range(0, carItemSpawnPos.Count);

        Instantiate(steeringWheel, carItemSpawnPos[spawn3], Quaternion.identity);
        carItemSpawnPos.RemoveAt(spawn3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
