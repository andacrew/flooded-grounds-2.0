using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHomePage : MonoBehaviour {

    public Animator indicatorAnimator;
    public Scrollbar homeScroll;

	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (homeScroll.value >= 0.7)
        {
            indicatorAnimator.Play("PPI 1 to 2");
        }
        else
        {
            indicatorAnimator.Play("PPI 2 to 1");
        }
	}
}
