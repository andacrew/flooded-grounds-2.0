using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour {

	[Header("OBJECTS")]
	public Text photoText;
	public Image photoViewer;
		
	[Header("VARIABLES")]
	public Sprite selectedPhotoUI;

	public void SetPhoto (Sprite selectedPhoto) 
	{
		photoViewer.sprite = selectedPhoto;
	}

	public void SetTitle (string photoTitle) 
	{
		photoText.text = photoTitle;
	}
}
