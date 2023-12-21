using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBackgroundWithResulation : MonoBehaviour {
	public Camera Cam;
	public GameObject WinloosScreen,SettingScreen, ObjectionScreen;
	int count;
	// Use this for initialization
	void Start () {
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (count == 1)
			return;
		if(Cam.aspect >= 1.777778f)
		{			
			this.gameObject.transform.localScale = new Vector3 (1.585f, 1.585f, 1.332811f);
			for(int i =0; i<WinloosScreen.transform.childCount; i++)
			{
				WinloosScreen.transform.GetChild (i).localScale = new Vector3(0.9f, 0.9f, 0.9f);
			}
			SettingScreen.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			ObjectionScreen.transform.GetChild (0).localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			ObjectionScreen.transform.GetChild (1).localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			count = 1;
		}else{
			
			this.gameObject.transform.localScale = new Vector3 (1.332811f, 1.332811f, 1.332811f);
			for(int i =0; i<WinloosScreen.transform.childCount; i++)
			{
				WinloosScreen.transform.GetChild (i).localScale = Vector3.one;
			}
			SettingScreen.transform.localScale = Vector3.one;
			ObjectionScreen.transform.GetChild (0).localScale = Vector3.one;
			ObjectionScreen.transform.GetChild (1).localScale = Vector3.one;
			count = 1;
		}

	}
}
