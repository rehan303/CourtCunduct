using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lawyer : MonoBehaviour {

	public Sprite CharClearSprite;
	public Sprite CharBlurSprite;
	// Use this for initialization
	void Start () {

	}

	public void CharacterForgroundAndBackGround(bool Charforground)
	{
		this.gameObject.SetActive (true);
		if (Charforground) {
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = CharClearSprite; 
			this.gameObject.GetComponent<Animator> ().enabled = true;
			this.gameObject.GetComponent<SpriteRenderer> ().flipX = false;
		
		}else
		{
			this.gameObject.GetComponent<Animator> ().enabled = false;
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = CharBlurSprite;	
			this.gameObject.GetComponent<SpriteRenderer> ().flipX = false;
		}
	}


}
