using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieMenuAnim : MonoBehaviour {

    [Header("ANIMATORS")]
    public Animator panelAnimator;

    [Header("ANIMATION STRINGS")]
    public string fadeInAnim;
    public string fadeOutAnim;

    [Header("SETTINGS")]
    public string shortcutKey;

    private bool isOn = false;
    private bool isHolding = false;

    void Update()
    {

        if (Input.GetKey(shortcutKey))
        {
            isHolding = true;
            isOn = false;
        }
        else
        {
            isHolding = false;
            isOn = true;
        }

        if (isOn == true && isHolding == false)
        {
            panelAnimator.Play(fadeOutAnim);
            isHolding = false;
            isOn = false;
        }
        else if (isOn == false && isHolding == true)
        {
            panelAnimator.Play(fadeInAnim);
            isHolding = true;
        }
    }

    public void AnimatePanel ()
    {
        if (isOn == true)
        {
            panelAnimator.Play(fadeOutAnim);
            isOn = false;
        }
        else if (isOn == false)
        {
            panelAnimator.Play(fadeInAnim);
            isOn = true;
        }
    }
}
