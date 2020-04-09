using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandler : MonoBehaviour {


    [Header("PANEL LIST")]
    public List<GameObject> panels = new List<GameObject>();

    [Header("BUTTON LIST")]
    public List<GameObject> buttons = new List<GameObject>();

    [Header("PANEL ANIMS")]
    public string panelFadeIn = "Panel Open";
    public string panelFadeOut = "Panel Close";

    [Header("BUTTON ANIMS")]
    public string buttonFadeIn = "TP Open";
    public string buttonFadeOut = "TP Close";

    private GameObject currentPanel;
    private GameObject nextPanel;

    private GameObject currentButton;
    private GameObject nextButton;

    [Header("SETTINGS")]
    public int currentPanelIndex = 0;
    private int currentButtonlIndex = 0;

    private Animator currentPanelAnimator;
    private Animator nextPanelAnimator;

    private Animator currentButtonAnimator;
    private Animator nextButtonAnimator;

    void Start ()
    {
        currentButton = buttons[currentButtonlIndex];
        currentButtonAnimator = currentButton.GetComponent<Animator>();
        currentButtonAnimator.Play(buttonFadeIn);
    }

    public void PanelAnim (int newPanel)
    {
        if (newPanel != currentPanelIndex)
        {
            currentPanel = panels[currentPanelIndex];

            currentPanelIndex = newPanel;
            nextPanel = panels[currentPanelIndex];

            currentPanelAnimator = currentPanel.GetComponent<Animator>();
            nextPanelAnimator = nextPanel.GetComponent<Animator>();

            currentPanelAnimator.Play(panelFadeOut);
            nextPanelAnimator.Play(panelFadeIn);

            currentButton = buttons[currentButtonlIndex];

            currentButtonlIndex = newPanel;
            nextButton = buttons[currentButtonlIndex];

            currentButtonAnimator = currentButton.GetComponent<Animator>();
            nextButtonAnimator = nextButton.GetComponent<Animator>();

            currentButtonAnimator.Play(buttonFadeOut);
            nextButtonAnimator.Play(buttonFadeIn);
        }
    }
}
