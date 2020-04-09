using UnityEngine;
using UnityEngine.UI;

public class SideMenu : MonoBehaviour {

	[Header("ANIMATORS")]
	public Animator sideMenuAnimator;

	private string menuOpening = "Side Menu Opening";
	private string menuClosing = "Side Menu Closing";

	[Header("VARIABLES")]
	public bool isOpen;

	void Start ()
	{
		if (isOpen == true)
			sideMenuAnimator.Play(menuOpening);
		else
			Debug.Log ("Side Menu Closed");
	}

	public void OpenClose()
	{
		if (isOpen == true) 
		{
			sideMenuAnimator.Play(menuClosing);
			isOpen = false;
		} 

		else 
		{
			sideMenuAnimator.Play(menuOpening);
			isOpen = true;
		}
	}
}