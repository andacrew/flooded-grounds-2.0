using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    public float alpha = 1;
    private CanvasGroup trans;

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<CanvasGroup>();
        trans.alpha = alpha;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
