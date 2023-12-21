using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectionControle : MonoBehaviour {
	public static ObjectionControle Instance = null; 
	public GameObject ObjectionType;
	public GameObject[] ObjectionTypeAns;
	public int Ans;
	public GameObject[] Button;
	Agrument tempArgu;
	public string JudgDecision;
	public GameObject JudgDecisionPanel;

	void Start()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != null) {
			Destroy (this.gameObject);
		}
		Button [0].SetActive (false);
		Button [1].SetActive (false);
	}

	public void SelectedAns(int ans)
	{
		Ans = ans;
		for(int i=0; i<ObjectionTypeAns.Length; i++)
		{
			if (i == Ans) {
				ObjectionTypeAns [i].gameObject.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
			} else
				ObjectionTypeAns [i].gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
			
		}
		Button [0].SetActive (true);
		Button [1].SetActive (false);
	}

	public void SubmitAns()
	{
		if(tempArgu.CorrectAns == Ans)
		{
			ObjectionTypeAns [Ans].gameObject.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
			ObjectionTypeAns [Ans].transform.GetChild (1).gameObject.SetActive (true);
			ObjectionTypeAns [Ans].transform.GetChild (2).gameObject.SetActive (false);

		}else
		{
			ObjectionTypeAns [Ans].gameObject.transform.localScale = new Vector3 (1f, 1f, 1f);
			ObjectionTypeAns [Ans].transform.GetChild (1).gameObject.SetActive (false);
			ObjectionTypeAns [Ans].transform.GetChild (2).gameObject.SetActive (true);

			ObjectionTypeAns [tempArgu.CorrectAns].gameObject.transform.localScale = new Vector3 (1.1f, 1.1f, 1.1f);
			ObjectionTypeAns [tempArgu.CorrectAns].transform.GetChild (1).gameObject.SetActive (true);
			ObjectionTypeAns [tempArgu.CorrectAns].transform.GetChild (2).gameObject.SetActive (false);
			if(GameManager.Instance.isVibration)
				Handheld.Vibrate();

		}
		Button [0].SetActive (false);
		Button [1].SetActive (true);
		for(int i=0; i<ObjectionTypeAns.Length; i++)
		{
			ObjectionTypeAns [i].GetComponent<Button> ().interactable = false;
		}

	}

	public void SetText(Agrument argu)
	{
		tempArgu = argu;
		Ans = -1;
//		ObjectionType.transform.GetComponentInChildren<Text> ().text = tempArgu.Prosecution[GameManager.Instance.ClickCount];
		for(int i = 0; i< tempArgu.ObjectionType.Length; i++)
		{
			ObjectionTypeAns [i].transform.GetComponentInChildren<Text> ().text = tempArgu.ObjectionType [i];
			ObjectionTypeAns [i].GetComponent<Button> ().interactable = true;

		}

		for(int i=0; i<ObjectionTypeAns.Length; i++)
		{
			ObjectionTypeAns [i].gameObject.transform.localScale = Vector3.one;
			ObjectionTypeAns [i].transform.GetChild (1).gameObject.SetActive (false);
			ObjectionTypeAns [i].transform.GetChild (2).gameObject.SetActive (false);
		}
		Button [0].SetActive (false);
		Button [1].SetActive (false);
	}

	public void SetTextForDefence(Agrument argu)
	{
		tempArgu = argu;
		Ans = -1;
//		ObjectionType.transform.GetComponentInChildren<Text> ().text = tempArgu.Defence[GameManager.Instance.ClickCount];
		for(int i = 0; i< tempArgu.ObjectionType.Length; i++)
		{
			ObjectionTypeAns [i].transform.GetComponentInChildren<Text> ().text = tempArgu.ObjectionType [i];
			ObjectionTypeAns [i].GetComponent<Button> ().interactable = true;

		}

		for(int i=0; i<ObjectionTypeAns.Length; i++)
		{
			ObjectionTypeAns [i].gameObject.transform.localScale = Vector3.one;
			ObjectionTypeAns [i].transform.GetChild (1).gameObject.SetActive (false);
			ObjectionTypeAns [i].transform.GetChild (2).gameObject.SetActive (false);
		}
		Button [0].SetActive (false);
		Button [1].SetActive (false);
	}


	public void SetAgrumentForDefence(Agrument argu)
	{
		GameScreenManager.Instance.BackgroundOnForGround (true);
		this.transform.GetChild (0).gameObject.SetActive (false);
		JudgDecisionPanel.SetActive (true);
		tempArgu = argu;
		this.transform.GetChild (1).transform.GetChild (3).transform.GetComponentInChildren<Text> ().text = argu.Defence[GameManager.Instance.ClickCount];
		JudgDecision = "";

	}

	public void SetAgrumentForProsecution(Agrument argu)
	{
		GameScreenManager.Instance.BackgroundOnForGround (true);
		this.transform.GetChild (0).gameObject.SetActive (false);
		JudgDecisionPanel.SetActive (true);
		tempArgu = argu;
		this.transform.GetChild (1).transform.GetChild (3).transform.GetComponentInChildren<Text> ().text = argu.Prosecution[GameManager.Instance.ClickCount];
		JudgDecision = "";

	}

	public void ObjectionDecision(string decision)
	{	

		if(tempArgu.Judge[0] == decision)
		{
			JudgDecision = decision;
		}else
		{
			JudgDecision = decision;
			if(GameManager.Instance.isVibration)
				Handheld.Vibrate();
		}
		JudgDecisionPanel.SetActive (false);
		this.transform.GetChild (0).gameObject.SetActive (false);
		GameManager.Instance.CaseAndLevelSelections ();
		GameScreenManager.Instance.BackgroundOnForGround (false);

	}

}
