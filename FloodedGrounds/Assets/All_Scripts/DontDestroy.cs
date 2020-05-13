
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake(){
        DontDestroyOnLoad(this.gameObject);
    }

    void Start(){
        DontDestroyOnLoad(this.gameObject);
    }
}

