using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using UnityEngine.Networking;

public class CaseSelectionManager : MonoBehaviour 
{
	public static CaseSelectionManager instance;

	[Header("Case1")]
	public List<CaseLevelScores> Case1 = new List<CaseLevelScores>();

	[Header("Case1Levels")]
	public GameObject[] LevelsObject;


	// Use this for initialization
	void Awake () 
	{
		if(instance == null)
		{
			instance = this;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
			
	}

	public IEnumerator GetSaveStateOfTheGame()
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);
		var encoding = new System.Text.UTF8Encoding ();
		Dictionary<string,string> postHeader = new Dictionary<string,string> ();

		UnityWebRequest request= UnityWebRequest.Get(DatabaseURL.GetSaveStateOfTheGame);
		request.SetRequestHeader("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		yield return request.SendWebRequest();
		Debug.Log("DATA is - " + request.downloadHandler.text);

		if (request.error == null)
		{
			var response = JSON.Parse(request.downloadHandler.text);
			if(response["statusCode"].ToString() == "200")
			{
				if(response["data"] == null)
				{
					Debug.Log ("New User");
				}
				else
				{
					for(int i = 0; i < response["data"].Count; i++)
					{
						for(int j = 0; j < response["data"][i]["levels"].Count; j++)
						{
							var levels = response ["data"] [i] ["levels"];
							CaseLevelScores temp = new CaseLevelScores ();
							temp.CaseID = response["data"][i]["case_id"].ToString ().Trim ('\"');
							temp.LevelID = levels[j]["level_id"].ToString ().Trim ('\"');
							int.TryParse (levels[j]["score"].Value, out temp.Scores);
							Case1.Add (temp);
						}
					}

					for(int i = 0; i < Case1.Count; i++)
					{
						LevelsObject [i].GetComponent<LevelSelection> ().SetValues (Case1[i]);
					}
				}
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
			else
			{
				Debug.Log(response["message"].ToString());
				PopUpManager.instance.ShowPopUp("ERROR!!", response["message"].ToString());
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
		}
		else
		{
			if(request.error == "400 Bad Request")
			{
				var response = JSON.Parse(request.downloadHandler.text);
				string messageOnSuccess = response["message"].ToString();
				Debug.Log(messageOnSuccess);
				PopUpManager.instance.ShowPopUp("ERROR!!", messageOnSuccess);
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
			else
			{
				Debug.Log("ERROR OCCURED!!");
				PopUpManager.instance.ShowPopUp("ERROR OCCURED!!", "Please try again Later");
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
		}
	}
}

[Serializable]
public class CaseLevelScores
{
	public string CaseID;
	public string LevelID;
	public int Scores = 0;
}
