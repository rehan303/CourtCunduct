using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour 
{

	public GameObject LevelNumberText;
	public GameObject LevelLockImage;
	public Image[] StarsAchieved;
	public Sprite FilledStar;
	public CaseLevelScores ThisLevel;
	private float TotalStars;

	// Use this for initialization
	void Start () 
	{
		
	}

	public void SetValues(CaseLevelScores levelParameters)
	{
		ThisLevel = levelParameters;
		LevelNumberText.SetActive (true);
		LevelLockImage.SetActive (false);
		TotalStars = (float)levelParameters.Scores/(100.0f/3.0f);
		int round = Mathf.FloorToInt (TotalStars);
		float floatValue = (float)TotalStars - (float)round;
		for (int i = 0; i < round; i++) 
		{
			StarsAchieved [i].fillAmount = 1;
		}
//		StarsAchieved [round].fillAmount = floatValue;
		// 
		if(levelParameters.Scores >= 60)
		{
			int level = 0;
			int.TryParse (levelParameters.LevelID, out level);
			CaseSelectionManager.instance.LevelsObject [level].GetComponent<LevelSelection> ().LevelNumberText.SetActive (true);
			CaseSelectionManager.instance.LevelsObject [level].GetComponent<LevelSelection> ().LevelLockImage.SetActive (false);
		}
	}


}
