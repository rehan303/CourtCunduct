using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour {

	// Use this for initialization
	public static HomeScreenManager instance;

	[SerializeField]
	private GameObject HomeScreen_CourtText;

	[SerializeField]
	private GameObject HomeScreen_ConductText;

	[SerializeField]
	private GameObject TargetPositionLeft_CourtText;
	[SerializeField]
	private GameObject TargetPositionCenter_CourtText;

	[SerializeField]
	private GameObject TargetPositionRight_ConductText;
	[SerializeField]
	private GameObject TargetPositionCenter_ConductText;

	[SerializeField]
	private Sprite[] HomeScreenLogoAnimation;

	[SerializeField]
	private GameObject HomeScreenLogoAnimation_GameObject;

	void Start () 
	{
		if(instance == null)
		{
			instance = this;
		}
//		StartCoroutine ("AnimationCourtConduct");
//		StartCoroutine ("PlayLogoAnimation");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public IEnumerator AnimationCourtConduct()
	{
		yield return new WaitForSeconds (0.1f);
		Debug.Log ("Animation is Playing");
		StartCoroutine (PlayLogoAnimation());
		iTween.MoveTo (HomeScreen_CourtText, iTween.Hash ("position",TargetPositionCenter_CourtText.transform.position,"time", 0.8f, "easeType", iTween.EaseType.linear));
		yield return new WaitForSeconds (0.8f);
		iTween.MoveTo (HomeScreen_ConductText, iTween.Hash ("position",TargetPositionCenter_ConductText.transform.position,"time", 0.8f, "easeType", iTween.EaseType.linear));
	}

	IEnumerator PlayLogoAnimation()
	{
		for(int i = 0; i < HomeScreenLogoAnimation.Length; i++)
		{
			HomeScreenLogoAnimation_GameObject.GetComponent<Image> ().sprite = HomeScreenLogoAnimation[i];
			yield return new WaitForSeconds (0.2f);
		}
	}

	public void ResetPositionForAnimations()
	{
		HomeScreen_CourtText.transform.position = TargetPositionLeft_CourtText.transform.position;
		HomeScreen_ConductText.transform.position = TargetPositionRight_ConductText.transform.position;
		HomeScreenLogoAnimation_GameObject.GetComponent<Image> ().sprite = HomeScreenLogoAnimation[0];

	}
}
