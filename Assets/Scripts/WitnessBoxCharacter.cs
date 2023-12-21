using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WitnessBoxCharacter : MonoBehaviour {
	public Sprite CharClearSprite;
	public Sprite CharBlurSprite;


	// Use this for initialization
	void Start () {
		
	}

	public void CharacterForgroundAndBackGround(bool Charforground)
	{
		this.gameObject.SetActive (true);
		if(this.gameObject.name.Contains ("Thomas"))
		{
			GameManager.Instance.isParker = false;
			GameManager.Instance.isThomos = true;
			GameManager.Instance.isBrewster = false;
			GameManager.Instance.isPeter= false;
		}else if(this.gameObject.name.Contains ("Parker"))
		{
			GameManager.Instance.isParker = true;
			GameManager.Instance.isThomos = true;
			GameManager.Instance.isBrewster = false;
			GameManager.Instance.isPeter= false;
		}else if(this.gameObject.name.Contains ("Peter"))
		{
			GameManager.Instance.isParker = false;
			GameManager.Instance.isThomos = false;
			GameManager.Instance.isBrewster = false;
			GameManager.Instance.isPeter= true;
		}else if(this.gameObject.name.Contains ("Brewster"))
		{
			GameManager.Instance.isParker = false;
			GameManager.Instance.isThomos = false;
			GameManager.Instance.isBrewster = true;
			GameManager.Instance.isPeter= false;
		}

		if (Charforground) {
			this.gameObject.GetComponent<Image> ().sprite = CharClearSprite; 
		}else
		{
			this.gameObject.GetComponent<Image> ().sprite = CharBlurSprite;
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
