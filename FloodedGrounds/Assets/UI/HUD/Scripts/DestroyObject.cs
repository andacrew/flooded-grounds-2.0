using UnityEngine;

public class DestroyObject : MonoBehaviour {

    public GameObject destroyObj;

	void Start ()
    {
        Destroy(destroyObj);
    }
}
