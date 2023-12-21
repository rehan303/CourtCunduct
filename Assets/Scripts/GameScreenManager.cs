using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenManager : MonoBehaviour {
	public static GameScreenManager Instance = null;

	public GameObject ObjectionScreen;
	public GameObject ArgumentScreen;
	public GameObject PauseScreen;
	public GameObject QuitScreen;
	public GameObject SettingScreen;
	public GameObject WinOrLoosScreen;
	public GameObject BackGround;
	public GameObject PauseButton;
	public Sprite[] BackGroundImage;

	[Header("Win Lose Screen Variable")]
	public Text GameLevelText;
	public Text CurrentLevelScoreText;
	public Sprite[] GameStatusImage;
	public Image BadgeHolder;

	public bool isExit;

	void Start()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != null) {
			Destroy (this.gameObject);
		}
	}

	public void BackgroundOnForGround(bool forground)
	{
		if (forground)
			BackGround.GetComponent<SpriteRenderer> ().sprite = BackGroundImage [0];
		else
			BackGround.GetComponent<SpriteRenderer> ().sprite = BackGroundImage [1];
	}

	public void ShowObjectionScreen()
	{
		ObjectionScreen.SetActive (true);
		ArgumentScreen.SetActive (false);
		PauseScreen.SetActive (false);
		QuitScreen.SetActive (false);
		SettingScreen.SetActive (false);
		WinOrLoosScreen.SetActive (false);
	}
	public void ShowArugmentScreen()
	{
		ObjectionScreen.SetActive (false);
		ArgumentScreen.SetActive (true);
		PauseScreen.SetActive (false);
		QuitScreen.SetActive (false);
		SettingScreen.SetActive (false);
		WinOrLoosScreen.SetActive (false);
	}
	public void PauseScreenShow()
	{
		if (GameManager.Instance.isPause) {
			PauseScreen.SetActive (true);
			PauseButton.SetActive (false);
		} else {
			PauseScreen.SetActive (false);
			PauseButton.SetActive (true);
		}
		QuitScreen.SetActive (false);
		SettingScreen.SetActive (false);
		WinOrLoosScreen.SetActive (false);
	}
	public void QuitScreenShow()
	{
		if(QuitScreen.activeInHierarchy){
			QuitScreen.SetActive (false);
			PauseScreen.SetActive (true);
		}else
		{
			QuitScreen.SetActive (true);
			PauseScreen.SetActive (false);
		}
		isExit = false;
		SettingScreen.SetActive (false);
		WinOrLoosScreen.SetActive (false);
	}
	public void SettingScreenShow()
	{
		if (SettingScreen.activeInHierarchy) {
			SettingScreen.SetActive (false);
			PauseScreen.SetActive (true);
		} else {
			SettingScreen.SetActive (true);
			PauseScreen.SetActive (false);
		}
		QuitScreen.SetActive (false);
		WinOrLoosScreen.SetActive (false);
	}
	public void WinOrLoosScreenShow()
	{
		ObjectionScreen.SetActive (false);
		ArgumentScreen.SetActive (false);
		PauseScreen.SetActive (false);
		QuitScreen.SetActive (false);
		SettingScreen.SetActive (false);
		WinOrLoosScreen.SetActive (true);
	}

	public void GameExit(string msg)
	{
		if(msg == "Yes")
		{
			PlayerPrefs.SetString (PrefPaths.RegistrationSceneBackFromGamePlay, "YES");
			UnityEngine.SceneManagement.SceneManager.LoadScene ("00_RegistrationScene");
			Time.timeScale = 1;
		}
		else if(msg == "No")
		{
			QuitScreenShow ();
		}
	}

	public void Replay()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene ("01_MainGamePlayScene");
		Time.timeScale = 1;

	}

	public void GameWinLoseStatus(string gameLevel, int score)
	{
		GameLevelText.text = gameLevel;
		CurrentLevelScoreText.text = score.ToString ();	
		WinOrLoosScreenShow ();
		// new changes
		GameManager.Instance.SelectedLawyer.SetActive (false);
		GameManager.Instance.TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive(false);
		GameManager.Instance.Ms_Brewster.SetActive(false);
		GameManager.Instance.Miss_Parker.SetActive (false);
		GameManager.Instance.Dr_Thomos.SetActive (false);
		GameManager.Instance.Mr_Peter.SetActive (false);
		if(score > 50)
		{
			BadgeHolder.sprite = GameStatusImage [0];
			// Loose Player
			for(int i = 0; i< GameManager.Instance.LoosScreenAdvocates.Length; i++)
			{
				if (GameManager.Instance.WinScreenAdvocates [i].name == PlayerPrefs.GetString ("SelectedProsecutor")) {
					GameManager.Instance.WinScreenAdvocates [i].SetActive (true);
				}else
					GameManager.Instance.WinScreenAdvocates [i].SetActive (false);
			}
			//Win Player
			int cont = -1;
			for (int i = 0; i< GameManager.Instance.LoosScreenAdvocates.Length; i++)
			{
				if (GameManager.Instance.LoosScreenAdvocates [i].name != PlayerPrefs.GetString ("SelectedProsecutor")) {
					cont++;
					GameManager.Instance.TempWinScreenAdvocates  [cont] =  GameManager.Instance.LoosScreenAdvocates[i];
				}			
			}

			for (int i = 0; i< GameManager.Instance.TempWinScreenAdvocates.Length; i++)
			{
				if(i == PlayerPrefs.GetInt ("Defender"))
				{
					GameManager.Instance.TempWinScreenAdvocates [i].SetActive (true);
				}else
					GameManager.Instance.TempWinScreenAdvocates [i].SetActive (false);
			}
		}else
		{
			BadgeHolder.sprite = GameStatusImage [1];
			// Loose Player
			for(int i = 0; i< GameManager.Instance.LoosScreenAdvocates.Length; i++)
			{
				if (GameManager.Instance.LoosScreenAdvocates [i].name == PlayerPrefs.GetString ("SelectedProsecutor")) {
					GameManager.Instance.LoosScreenAdvocates [i].SetActive (true);
				}else
					GameManager.Instance.LoosScreenAdvocates [i].SetActive (false);
			}
			//Win Player
			int cont = -1;
			for (int i = 0; i< GameManager.Instance.LoosScreenAdvocates.Length; i++)
			{
				if (GameManager.Instance.LoosScreenAdvocates [i].name != PlayerPrefs.GetString ("SelectedProsecutor")) {
					cont++;
					GameManager.Instance.TempWinScreenAdvocates  [cont] =  GameManager.Instance.LoosScreenAdvocates[i];
				}			
			}

			for (int i = 0; i< GameManager.Instance.TempWinScreenAdvocates.Length; i++)
			{
				if(i == PlayerPrefs.GetInt ("Defender"))
				{
					GameManager.Instance.TempWinScreenAdvocates [i].SetActive (true);
					GameManager.Instance.TempWinScreenAdvocates [i].transform.localPosition = new Vector3 (-418f, -66f, 0);
				}else
					GameManager.Instance.TempWinScreenAdvocates [i].SetActive (false);
			}
		}
	
	}

}
