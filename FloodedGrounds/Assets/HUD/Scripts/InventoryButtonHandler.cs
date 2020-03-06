using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class InventoryButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ANIMATORS")]
    public Animator objectAnimator;

    private bool isOpen = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isOpen == false)
        {
            objectAnimator.Play("IB Expand");
            isOpen = true;
        }
        else if (isOpen == true)
        {
            objectAnimator.Play("IB Minimize");
            isOpen = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        objectAnimator.Play("IB Minimize");
        isOpen = false;
    }

    public void TriggerExit()
    {
        objectAnimator.Play("IB Minimize");
        isOpen = false;
    }
}
