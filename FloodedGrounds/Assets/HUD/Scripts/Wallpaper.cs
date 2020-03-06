using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wallpaper : MonoBehaviour {

    [Header("WALLPAPERS")]
    public Image wallpaperObject;
    public List<Sprite> ImageList = new List<Sprite>();
    public Sprite currentSprite;

    private int wallpaperPrefs;
    private int currentImageIndex = 0;

    void Start ()
    {
        wallpaperPrefs = PlayerPrefs.GetInt("Wallpaper");
        currentImageIndex = wallpaperPrefs;
        currentSprite = ImageList[currentImageIndex];
        wallpaperObject.sprite = currentSprite;
    }
	
	public void setWallpaper (int WallpaperIndex)
    {
        currentImageIndex = WallpaperIndex;
        currentSprite = ImageList[currentImageIndex];
        wallpaperObject.sprite = currentSprite;
        PlayerPrefs.SetInt("Wallpaper", WallpaperIndex);
    }
}
