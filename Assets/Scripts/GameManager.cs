using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;
	public ArgumentTurn ArgueTurn;
	public string Case;
	public int CaseLevel;
	public bool isPause;
	public bool isObjection;
	public bool isRaisedObjection;
	public float letterPause = 0.0f;
	public int CurruntArguStep;
	public bool BackPressed;

	[Header ("Sounds Atributes")]
	public Slider MusicSlider;
	public Slider SoundSlider;
	public AudioSource Sound;
	public GameObject VibrationToggal, TutorialToggal;
	public bool isVibration, isTutorial;

	[Header ("Advocates")]
	public GameObject[] Advocates;
	public GameObject[] TempAdvocates;
	public GameObject[] WinScreenAdvocates;
	public GameObject[] TempWinScreenAdvocates;
	public GameObject[] LoosScreenAdvocates;
	[Header ("Selected Lawyer")]
	public GameObject SelectedLawyer;
	[Header ("Victim Char")]
	public GameObject Miss_Parker;
	[Header ("Victim Witness")]
	public GameObject Ms_Brewster;
	[Header ("Offender Char")]
	public GameObject Dr_Thomos;
	[Header ("Offender Witness")]
	public GameObject Mr_Peter;
	[Header ("Level Introduction")]
	public List<LevelIntro> LevelIntroduction = new List<LevelIntro> ();
	[Header ("Case 1 Level1")]
	public List<Agrument> Case1_Level1_Argument = new List<Agrument> ();
	[Header ("Case 1 Level2")]
	public List<Agrument> Case1_Level2_Argument = new List<Agrument> ();
	[Header ("Case 1 Level3")]
	public List<Agrument> Case1_Level3_Argument = new List<Agrument> ();
	[Header ("Case 1 Level4")]
	public List<Agrument> Case1_Level4_Argument = new List<Agrument> ();
	[Header ("Argumet Text")]
	public GameObject LawyerTextBox;
	public GameObject VictimTextBox;
	public GameObject DefinationBox;
	public GameObject ObjectionPanel;
	public GameObject ObjectionButton;
	public GameObject IntroductionPanel;
	public GameObject InvalidObjection;

	public Button BackButton1;
	public Button BackButton2;


	[Header ("Witness Box Char")]
	public bool isParker;
	public bool isThomos;
	public bool isBrewster;
	public bool isPeter;

	public int ClickCount = -1;
	public bool isNext;
	public bool isScroll;

	// Use this for initialization
	void Start ()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != null) {
			Destroy (this.gameObject);
		}
		CurruntArguStep = 0;
		BackPressed = false;
		ToggalAndSoundState ();
		ArgueTurn = ArgumentTurn.Null;
		isNext = true;
		isObjection = false;
		SelectProsecutor ();
		Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
		SelectDefenderProsecutor ();
		Case = PlayerPrefs.GetString ("Case");
		CaseLevel = PlayerPrefs.GetInt ("SelectedLevel");
		LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
		VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
		DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
		SetIntroductionForLevel ();
		PlayerPrefs.DeleteKey (PrefPaths.Score);
//		StartCoroutine (GameManager.Instance.SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
	}

	// Update is called once per frame
	void Update ()
	{

		if (isNext && !isPause && !isObjection) {
			if (Input.GetMouseButtonDown (0)) {	
				RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
				if (hit.collider != null) {
					if (hit.collider.gameObject.tag == "TextBox" && LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical
					    || hit.collider.gameObject.tag == "TextBox" && VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical
					    || hit.collider.gameObject.tag == "TextBox" && DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical) {
						isScroll = true;
					} else {
						isScroll = false;
					}			
				} else {	
					isScroll = false;
				}
				if (!isScroll && !hit.collider) {
					CaseAndLevelSelections ();					
				}
			}
			if (Input.GetMouseButtonUp (0)) {	
				isScroll = false;
			}
		}

		if (BackPressed) {
			BackButton1.transform.gameObject.SetActive (false);
			BackButton2.transform.gameObject.SetActive (false);
		}
	}

	// Select Prosecutor
	void SelectProsecutor ()
	{
		int randomValue = 0;
		randomValue = UnityEngine.Random.Range (0, 4);
		if (randomValue == 0)
			PlayerPrefs.SetString ("SelectedProsecutor", "Advocate1");
		else if (randomValue == 1)
			PlayerPrefs.SetString ("SelectedProsecutor", "Advocate2");
		else if (randomValue == 2)
			PlayerPrefs.SetString ("SelectedProsecutor", "Advocate3");
		else if (randomValue == 3)
			PlayerPrefs.SetString ("SelectedProsecutor", "Advocate4");
		
		for (int i = 0; i < Advocates.Length; i++) {
			if (Advocates [i].name == PlayerPrefs.GetString ("SelectedProsecutor")) {
				SelectedLawyer = Advocates [i];
			}
		}
	}

	void SelectDefenderProsecutor ()
	{
		int cont = -1;
		for (int i = 0; i < Advocates.Length; i++) {
			if (Advocates [i].name != PlayerPrefs.GetString ("SelectedProsecutor")) {
				cont++;
				TempAdvocates [cont] = Advocates [i];
			}			
		}
		PlayerPrefs.SetInt ("Defender", UnityEngine.Random.Range (0, 3));
		TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (true);
		TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
	}

	bool isBack;
	public bool inNextConversation;

	#region Case1 Arguments

	// Prosecution And Parker Argument
	IEnumerator ProsecutionAndParker_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Prosecution;
		if (ArgueTurn == ArgumentTurn.Prosecution) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			SelectedLawyer.SetActive (false);
			Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);

			Ms_Brewster.SetActive (false);
			Dr_Thomos.SetActive (false);
			Mr_Peter.SetActive (false);
					
			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);	
		

		} else if (ArgueTurn == ArgumentTurn.Miss_Parker) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
			SelectedLawyer.SetActive (false);
			Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Ms_Brewster.SetActive (false);
			Dr_Thomos.SetActive (false);
			Mr_Peter.SetActive (false);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Miss_Parker [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}
			
		if (ArgueTurn == ArgumentTurn.Prosecution) {

			BackButton1.transform.gameObject.SetActive (false);
			BackButton2.transform.gameObject.SetActive (true);
			if (CurruntArguStep == 0 && curruntAgru.Prosecution [ClickCount] == "Miss Parker, I know this is difficult, but can I ask that you please tell the court exactly what happened on the night of 6th November, 2015?"
			    || CurruntArguStep == 9 && curruntAgru.Prosecution [ClickCount] == "Miss Parker, in your mind is there any way Dr Thomas could have perceived the incident as a consensual act?"
			    || CurruntArguStep == 11 && curruntAgru.Prosecution [ClickCount] == "Miss Parker, in your mind is there any way Dr Thomas could have perceived the incident as a consensual act?") {
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (false);
			}		
			BackButton1.onClick.RemoveAllListeners ();
			BackButton1.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Prosecution;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();	
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;
			});

			if (ClickCount == curruntAgru.Prosecution.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);	
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});

				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;
				inNextConversation = false;

			} else {
				ArgueTurn = ArgumentTurn.Null;
				ArgueTurn = ArgumentTurn.Miss_Parker;
				ClickCount--;
				inNextConversation = false;
			}
		
		} else {
			ArgueTurn = ArgumentTurn.Prosecution;
			inNextConversation = true;
			if (ClickCount == curruntAgru.Miss_Parker.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			} else {
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			}

		}
		isNext = true;
	
	}



	// Prosecution Objection
	IEnumerator ProsecutionObjection_TextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Prosecution;

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}	

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Objection) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}	
			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [0].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;

			//Objection Box
			DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
			if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			if (ClickCount == curruntAgru.Prosecution.Length - 1) {
				if (CurruntArguStep == 11 && curruntAgru.Prosecution [ClickCount] == "OBJECTION- Hearsay") {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

					});

				} else {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

					});
				}
				inNextConversation = false;
			} 
			inNextConversation = false;
			ArgueTurn = ArgumentTurn.Objection;
			ClickCount--;
		} else {
			inNextConversation = true;
			ArgueTurn = ArgumentTurn.Null;
			ClickCount = -1;
			curruntAgru.Compleated = true;
			BackPressed = true;
		}


		isNext = true;    
	}


	// Defence And Parker Argument
	IEnumerator DefenceAndParker_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Defence;

		if (ArgueTurn == ArgumentTurn.Defence) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
			Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Ms_Brewster.SetActive (false);
			Dr_Thomos.SetActive (false);
			Mr_Peter.SetActive (false);

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Miss_Parker) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
			Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Miss_Parker [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Defence) {
			BackButton2.transform.gameObject.SetActive (false);
			BackButton1.transform.gameObject.SetActive (true);
			if (CurruntArguStep == 3 && curruntAgru.Defence [ClickCount] == "Do you think you have become skilled at rejecting men’s advances when they are unwanted?"
			    || CurruntArguStep == 5 && curruntAgru.Defence [ClickCount] == "Apologies. It’s just that if there is nothing physical evidence to support Ms Parker’s allegation that she resisted my client’s advances, how could the jury determine that the act was not consensual as my client says it was and how could my client have even known it was not consensual since as Miss Parker states she did not say no and did not resist?"
			    || CurruntArguStep == 5 && curruntAgru.Defence [ClickCount] == "Do you think you have become skilled at rejecting men’s advances when they are unwanted?"
			    || CurruntArguStep == 7 && curruntAgru.Defence [ClickCount] == "Apologies. It’s just that if there is nothing physical evidence to support Ms Parker’s allegation that she resisted my client’s advances, how could the jury determine that the act was not consensual as my client says it was and how could my client have even known it was not consensual since as Miss Parker states she did not say no and did not resist?") {
				BackPressed = true;
			}
			BackButton2.onClick.RemoveAllListeners ();
			BackButton2.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Defence;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();

				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;
			});
			if (ClickCount == curruntAgru.Defence.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton1.transform.gameObject.SetActive (true);

				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (false);
				});
				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;
			} else {
				ArgueTurn = ArgumentTurn.Miss_Parker;
				ClickCount--;
			}
		} else {
			ArgueTurn = ArgumentTurn.Defence;
			inNextConversation = true;
			if (ClickCount == curruntAgru.Miss_Parker.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			} else {
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			}
		}
		isNext = true;    
	}

	// Defence and DrBrown Argument
	IEnumerator DrBrownAndDefence_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Defence;

		if (ArgueTurn == ArgumentTurn.Defence) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Ms_Brewster.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Thomas_Brown) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Ms_Brewster.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Thomas_Brown [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}	

		if (ArgueTurn == ArgumentTurn.Defence) {
			BackButton2.transform.gameObject.SetActive (false);
			BackButton1.transform.gameObject.SetActive (true);
			BackButton2.onClick.RemoveAllListeners ();
			BackButton2.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Defence;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();	
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;

			});
			if (ClickCount == curruntAgru.Defence.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				if (CurruntArguStep == 10 && curruntAgru.Defence [ClickCount] == "And how do you know that Miss Parker was not upset the day after the alleged attack?") {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
				} else {
					ArgueTurn = ArgumentTurn.Null;
					ClickCount = -1;
					curruntAgru.Compleated = true;
				}
			} else {
				ArgueTurn = ArgumentTurn.Thomas_Brown;
				ClickCount--;
			}
		} else {			
			ArgueTurn = ArgumentTurn.Defence;
			inNextConversation = true;
			if (ClickCount == curruntAgru.Thomas_Brown.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				if (CurruntArguStep == 12 && curruntAgru.Thomas_Brown [ClickCount] != "It may sound cliche but I have always wanted to help people. I’ve done a lot of volunteer work in Africa as well. I give a lot to charity also."
				    || CurruntArguStep == 10 && curruntAgru.Thomas_Brown [ClickCount] == "After I was charged a colleague of mine told me that he had gone to the club the next day and that when mentioned that he knew me, she said she hoped I would be back to the club again sometime soon.") {
					ArgueTurn = ArgumentTurn.Null;
					ClickCount = -1;
					curruntAgru.Compleated = true;
				}
			} else {
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			}

		}
		isNext = true;
	}

	// Prosecution And Thomos Argument
	IEnumerator ProsecutionAndThomos_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Prosecution;

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Ms_Brewster.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);
			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Thomas_Brown) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);		
			SelectedLawyer.SetActive (false);
			Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Ms_Brewster.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Thomas_Brown [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			BackButton1.transform.gameObject.SetActive (false);
			BackButton2.transform.gameObject.SetActive (true);
			if (CurruntArguStep == 12 && curruntAgru.Prosecution [ClickCount] == "When was the last time you and your wife had sex?"
			    || CurruntArguStep == 14 && curruntAgru.Prosecution [ClickCount] == "What is your opinion of exotic dancers?"
			    || CurruntArguStep == 16 && curruntAgru.Prosecution [ClickCount] == "Do you think that exotic dancers are already selling their bodies and this means that they can’t say no?"
			    || CurruntArguStep == 16 && curruntAgru.Prosecution [ClickCount] == "When was the last time you and your wife had sex?"
			    || CurruntArguStep == 18 && curruntAgru.Prosecution [ClickCount] == "What is your opinion of exotic dancers?"
			    || CurruntArguStep == 20 && curruntAgru.Prosecution [ClickCount] == "Do you think that exotic dancers are already selling their bodies and this means that they can’t say no?") {
				BackPressed = true;
			}
			BackButton1.onClick.RemoveAllListeners ();
			BackButton1.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Prosecution;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();	
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;

			});
			if (ClickCount == curruntAgru.Prosecution.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);

				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;
			} else {
				ArgueTurn = ArgumentTurn.Thomas_Brown;
				ClickCount--;
			}
		} else {
			ArgueTurn = ArgumentTurn.Prosecution;
			inNextConversation = true;
			if (ClickCount == curruntAgru.Thomas_Brown.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
			} else {
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
			}
		}
		isNext = true;    
	}

	// Defence Objection
	IEnumerator DefenceObjection_TextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Defence;

		if (ArgueTurn == ArgumentTurn.Defence) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);		
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}		

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Objection) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (true);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}
			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [0].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;

			//Objection Box
			DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
			if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Defence) {
			if (ClickCount == curruntAgru.Defence.Length - 1) {

				if (CurruntArguStep == 18 && curruntAgru.Defence [ClickCount] == "OBJECTION - Tendency evidence.") {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

					});
				} else {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

					});


				}
				inNextConversation = false;
			} 
			inNextConversation = false;
			ArgueTurn = ArgumentTurn.Objection;
			ClickCount--;
		} else {
			ArgueTurn = ArgumentTurn.Null;
			ClickCount = -1;
			curruntAgru.Compleated = true;
			BackPressed = true;
		}
		isNext = true;    
	}

	// Prosecution And Ms_Brewster Argument
	IEnumerator ProsecutionAndMs_Brewster_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Prosecution;

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Dr_Thomos.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Ms_Brewster) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Dr_Thomos.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Ms_Brewster [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			BackButton2.transform.gameObject.SetActive (false);
			BackButton1.transform.gameObject.SetActive (true);
			if (CurruntArguStep == 23 && curruntAgru.Prosecution [ClickCount] == "Could you elaborate on that side of Dr Brown you didn’t like?") {
				BackPressed = true;
			}
			BackButton2.onClick.RemoveAllListeners ();
			BackButton2.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Prosecution;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();	
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;

			});
			if (ClickCount == curruntAgru.Prosecution.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;
			} else {
				ArgueTurn = ArgumentTurn.Ms_Brewster;
				ClickCount--;
			}
		} else {
			ArgueTurn = ArgumentTurn.Prosecution;
			inNextConversation = true;
			if (ClickCount == curruntAgru.Ms_Brewster.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			} else {
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			}
		}
		isNext = true;    
	}

	// Defence And Ms_Brewster Argument
	IEnumerator DefenceAndMs_Brewster_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Defence;

		if (ArgueTurn == ArgumentTurn.Defence) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
			Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Miss_Parker.SetActive (false);
			Dr_Thomos.SetActive (false);
			Mr_Peter.SetActive (false);

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		} else if (ArgueTurn == ArgumentTurn.Ms_Brewster) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);		
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
			Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Miss_Parker.SetActive (false);
			Dr_Thomos.SetActive (false);
			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Ms_Brewster [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Defence) {
			BackButton1.transform.gameObject.SetActive (false);
			BackButton2.transform.gameObject.SetActive (true);
			if (CurruntArguStep == 19 && curruntAgru.Defence [ClickCount] == "Ms Brewster, you say that you felt pressured by Dr Brown to perform certain acts but did he ever actually force you to do anything expressly against your will?"
			    || CurruntArguStep == 25 && curruntAgru.Defence [ClickCount] == "Ms Brewster, you say that you felt pressured by Dr Brown to perform certain acts but did he ever actually force you to do anything expressly against your will?") {
				BackPressed = true;
			}
			BackButton1.onClick.RemoveAllListeners ();
			BackButton1.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Defence;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();		
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;
			});
			if (ClickCount == curruntAgru.Defence.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);

				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;
			} else {
				ArgueTurn = ArgumentTurn.Ms_Brewster;
				ClickCount--;
			}
		} else {
			ArgueTurn = ArgumentTurn.Defence;
			inNextConversation = true;
			if (ClickCount == curruntAgru.Ms_Brewster.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			} else {
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
			}
		}
		isNext = true;    
	}



	#endregion

	#region Case2 Arguments

	// Prosecution Objection In Level 2
	IEnumerator ProsecutionObjectionOption_TextShow (Agrument curruntAgru)
	{

		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Prosecution;

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (true);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}

			ObjectionControle.Instance.SetText (curruntAgru);
			isObjection = true;

		} else if (ArgueTurn == ArgumentTurn.Objection) {
			if (isObjection)
				isObjection = false;
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}	
			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [0].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;

			//Objection Box
			DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
			if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			if (ClickCount == curruntAgru.Prosecution.Length - 1) {

				if (CurruntArguStep == 11 && curruntAgru.Prosecution [ClickCount] == "OBJECTION What is the relevant legal reasoning for this objection?") {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
				} else {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
				}

				inNextConversation = false;
			} 
			inNextConversation = false;
			ArgueTurn = ArgumentTurn.Objection;
			ClickCount--;
		} else {
			ArgueTurn = ArgumentTurn.Null;
			ClickCount = -1;
			curruntAgru.Compleated = true;
			BackPressed = true;
		}
		isNext = true;    
	}


	// Defence Objection Level 2
	IEnumerator DefenceObjectionOption_TextShow (Agrument curruntAgru)
	{		
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Defence;

		if (ArgueTurn == ArgumentTurn.Defence) {
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (true);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}		

			ObjectionControle.Instance.SetTextForDefence (curruntAgru);
			isObjection = true;
		} else if (ArgueTurn == ArgumentTurn.Objection) {
			if (isObjection)
				isObjection = false;
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (true);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}
			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [0].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;

			//Objection Box
			DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
			if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Defence) {
			if (ClickCount == curruntAgru.Defence.Length - 1) {

				if (CurruntArguStep == 18 && curruntAgru.Defence [ClickCount] == "OBJECTION What is the relevant legal reasoning for this objection?") {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

					});

				} else {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

					});

				}
				inNextConversation = false;
			} 
			inNextConversation = false;
			ArgueTurn = ArgumentTurn.Objection;
			ClickCount--;
		} else {
			ArgueTurn = ArgumentTurn.Null;
			ClickCount = -1;
			curruntAgru.Compleated = true;
			BackPressed = true;
		}
		isNext = true;    
	}


	#endregion

	#region Case3 Arguments

	//  First Parker And Prosecution
	IEnumerator ProsecutionAndParkerFirst_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Miss_Parker;

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			SelectedLawyer.SetActive (false);
			Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Ms_Brewster.SetActive (false);
			Dr_Thomos.SetActive (false);
			Mr_Peter.SetActive (false);

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Miss_Parker) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
			SelectedLawyer.SetActive (false);
			Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Ms_Brewster.SetActive (false);
			Dr_Thomos.SetActive (false);
			Mr_Peter.SetActive (false);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Miss_Parker [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Miss_Parker) {
			inNextConversation = true;
			if (ClickCount == curruntAgru.Miss_Parker.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			} else {
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				if (CurruntArguStep == 2 && curruntAgru.Miss_Parker [ClickCount] == "Oh yes. Sorry. It was Dr Thomas Brown and his friend’s name was Peter. So we were talking for, I dunno, maybe half an hour, talking and drinking. Having some laughs. I was really enjoying their company actually. It’s nice when men come in and don’t instantly want you to dance for them. It makes you feel more appreciated I guess. Like they see you as more than a stripper. That’s how it felt anyway this night. Then Peter mentioned he had to get going and it was just him and me left. ") {
					BackPressed = true;
				}
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	

					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			}
			ArgueTurn = ArgumentTurn.Prosecution;
			ClickCount--;
		} else {
			ArgueTurn = ArgumentTurn.Miss_Parker;
			BackButton1.transform.gameObject.SetActive (false);
			BackButton2.transform.gameObject.SetActive (true);
			BackButton1.onClick.RemoveAllListeners ();
			BackButton1.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Prosecution;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();	
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;
			});

			if (ClickCount == curruntAgru.Prosecution.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				if (curruntAgru.Prosecution.Length > 2) {
					if (curruntAgru.Prosecution [2] == "Thank you Miss Parker.") {
						BackButton1.transform.gameObject.SetActive (false);
						BackButton2.transform.gameObject.SetActive (true);
					}
				}
				if (curruntAgru.Prosecution [3] == "Thank you Miss Parker, I know that must have been very difficult for you to recount.") {
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
				}
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;

			}
		}
		isNext = true;

	}

	// Prosecution Objection In Level 3
	IEnumerator JudgeDecision_ProsecutionObjection_TextShow (Agrument curruntAgru)
	{

		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Prosecution;

		if (ArgueTurn == ArgumentTurn.Prosecution) {			
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}	

			ObjectionControle.Instance.SetAgrumentForProsecution (curruntAgru);
			isObjection = true;

		} else if (ArgueTurn == ArgumentTurn.Objection) {
			if (isObjection)
				isObjection = false;
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
			SelectedLawyer.SetActive (false);
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}

			//Objection Box
			DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
			if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Prosecution) {
			if (ClickCount == curruntAgru.Prosecution.Length - 1) {
//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				inNextConversation = false;
			} 
			inNextConversation = false;
			ArgueTurn = ArgumentTurn.Objection;
			ClickCount--;
		} else {
			ArgueTurn = ArgumentTurn.Null;
			ClickCount = -1;
			curruntAgru.Compleated = true;
			BackPressed = true;
		}
		isNext = true;    
	}

	// Defence and DrBrown Argument First
	IEnumerator FirstDrBrownAndDefence_argumentTextShow (Agrument curruntAgru)
	{
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Thomas_Brown;

		if (ArgueTurn == ArgumentTurn.Defence) {
			LawyerTextBox.SetActive (true);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);

			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
			Ms_Brewster.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
			if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);

		} else if (ArgueTurn == ArgumentTurn.Thomas_Brown) {
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (true);
			DefinationBox.SetActive (false);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
			Ms_Brewster.SetActive (false);
			Miss_Parker.SetActive (false);
			Mr_Peter.SetActive (false);

			VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Thomas_Brown [ClickCount].ToString ();
			if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}	

		if (ArgueTurn == ArgumentTurn.Thomas_Brown) {
			inNextConversation = true;
			if (ClickCount + 1 == curruntAgru.Thomas_Brown.Length) {			
				PlayerPrefs.SetInt ("LastConversation", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount = PlayerPrefs.GetInt ("LastConversation");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});

				ClickCount = -1;
				curruntAgru.Compleated = true;
				ArgueTurn = ArgumentTurn.Null;
			} else {
				if (CurruntArguStep == 14 && curruntAgru.Thomas_Brown [ClickCount] == "She had been all over me all night, flirting, giggling, touching me.") {
					BackPressed = true;
				}
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				ArgueTurn = ArgumentTurn.Defence;
				ClickCount--;
			}
		} else {			
			ArgueTurn = ArgumentTurn.Thomas_Brown;
			BackButton1.transform.gameObject.SetActive (false);
			BackButton2.transform.gameObject.SetActive (true);
			BackButton1.onClick.RemoveAllListeners ();
			BackButton1.onClick.AddListener (() => {
				ArgueTurn = ArgumentTurn.Defence;
				if (inNextConversation) {
					ClickCount--;
					print (ClickCount);
				}
				CaseAndLevelSelectionsBackCase ();	
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				isObjection = false;

			});
			if (ClickCount == curruntAgru.Defence.Length - 1) {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
			}
		}


		isNext = true;
	}


	// Defence Objection In Level 3
	IEnumerator JudgeDecision_DefenceObjectionOption_TextShow (Agrument curruntAgru)
	{		
		if (ArgueTurn == ArgumentTurn.Null)
			ArgueTurn = ArgumentTurn.Defence;

		if (ArgueTurn == ArgumentTurn.Defence) {
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (false);
			// Change this line 
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}		

			ObjectionControle.Instance.SetAgrumentForDefence (curruntAgru);
			isObjection = true;
		} else if (ArgueTurn == ArgumentTurn.Objection) {
			if (isObjection)
				isObjection = false;
			ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
			LawyerTextBox.SetActive (false);
			VictimTextBox.SetActive (false);
			DefinationBox.SetActive (true);
			SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
			TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
			if (isParker) {
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isBrewster) {
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isThomos) {
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Mr_Peter.SetActive (false);
			} else if (isPeter) {
				Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
			}

			//Objection Box
			DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
			if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
			else
				DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
			yield return new WaitForSeconds (letterPause);
		}

		if (ArgueTurn == ArgumentTurn.Defence) {
			if (ClickCount == curruntAgru.Defence.Length - 1) {

				if (CurruntArguStep == 22 && curruntAgru.Defence [ClickCount] == "Badgering The Witness." || CurruntArguStep == 24 && curruntAgru.Defence [ClickCount] == "Tendency Evidence") {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
				} else {
					//				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						//					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
				}
				inNextConversation = false;
			} 
			inNextConversation = false;
			ArgueTurn = ArgumentTurn.Objection;
			ClickCount--;
		} else {
			ArgueTurn = ArgumentTurn.Null;
			ClickCount = -1;
			curruntAgru.Compleated = true;
			BackPressed = true;
		}
		isNext = true;    
	}


	#endregion

	#region Case4 Arguments

	// Prosecution And Parker Argument Level 4
	IEnumerator ProsecutionAndParker_argumentTextShowLevel4 (Agrument curruntAgru)
	{
		if (isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				SelectedLawyer.SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				SelectedLawyer.SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Prosecution;
			if (ArgueTurn == ArgumentTurn.Prosecution) {
				LawyerTextBox.SetActive (true);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				SelectedLawyer.SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
				
				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);

			} else if (ArgueTurn == ArgumentTurn.Miss_Parker) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (true);
				DefinationBox.SetActive (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
				SelectedLawyer.SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);

				VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Miss_Parker [ClickCount].ToString ();
				if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			}

			if (ArgueTurn == ArgumentTurn.Prosecution) {
				
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);
				if (CurruntArguStep == 0 && curruntAgru.Prosecution [ClickCount] == "Miss Parker, I know this is difficult, but can I ask that you please tell the court exactly what happened on the night of 6th November, 2015?"
				    || CurruntArguStep == 9 && curruntAgru.Prosecution [ClickCount] == "Miss Parker, in your mind is there any way Dr Thomas could have perceived the incident as a consensual act?") {
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (false);
				}
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					if (inNextConversation) {
						ClickCount--;
						print (ClickCount);
					}
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				});
				if (ClickCount == curruntAgru.Prosecution.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);

					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						isObjection = false;
						isRaisedObjection = false;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);

					});
					ClickCount = -1;
					curruntAgru.Compleated = true;
					ArgueTurn = ArgumentTurn.Null;
					inNextConversation = false;
				} else {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount--;
					inNextConversation = false;
				}
			} else {
				ArgueTurn = ArgumentTurn.Prosecution;
				inNextConversation = true;
				if (ClickCount == curruntAgru.Miss_Parker.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation", ClickCount);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Miss_Parker;
						ClickCount = PlayerPrefs.GetInt ("LastConversation");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					});
				} else {
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Miss_Parker;
						ClickCount--;
						print (ClickCount);
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					});
				}
			}
			isNext = true;
		}

	}





	// Defence And Parker Argument Leve 4
	IEnumerator DefenceAndParker_argumentTextShowLevel4 (Agrument curruntAgru)
	{
		if (isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Defence;

			if (ArgueTurn == ArgumentTurn.Defence) {
				LawyerTextBox.SetActive (true);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				// Change this line 
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);

				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);

			} else if (ArgueTurn == ArgumentTurn.Miss_Parker) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (true);
				DefinationBox.SetActive (false);
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);

				VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Miss_Parker [ClickCount].ToString ();
				if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			}

			if (ArgueTurn == ArgumentTurn.Defence) {
				BackButton2.transform.gameObject.SetActive (false);
				BackButton1.transform.gameObject.SetActive (true);
				if (CurruntArguStep == 3 && curruntAgru.Defence [ClickCount] == "Do you think you have become skilled at rejecting men’s advances when they are unwanted?"
				    || CurruntArguStep == 5 && curruntAgru.Defence [ClickCount] == "Apologies. It’s just that if there is nothing physical evidence to support Ms Parker’s allegation that she resisted my client’s advances, how could the jury determine that the act was not consensual as my client says it was and how could my client have even known it was not consensual since as Miss Parker states she did not say no and did not resist?") {
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (false);
					BackPressed = true;
				}
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					if (inNextConversation) {
						ClickCount--;
						print (ClickCount);
					}
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				});
				if (ClickCount == curruntAgru.Defence.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					});
					if (CurruntArguStep == 3 && curruntAgru.Defence [ClickCount] == "Do you think you have become skilled at rejecting men’s advances when they are unwanted?"
					    || CurruntArguStep == 5 && curruntAgru.Defence [ClickCount] == "Apologies. It’s just that if there is nothing physical evidence to support Ms Parker’s allegation that she resisted my client’s advances, how could the jury determine that the act was not consensual as my client says it was and how could my client have even known it was not consensual since as Miss Parker states she did not say no and did not resist?") {
						BackButton2.transform.gameObject.SetActive (false);
						BackButton1.transform.gameObject.SetActive (false);
						BackPressed = true;
					}
					ClickCount = -1;
					curruntAgru.Compleated = true;
					ArgueTurn = ArgumentTurn.Null;
					inNextConversation = false;
				} else {
					ArgueTurn = ArgumentTurn.Miss_Parker;
					ClickCount--;
					inNextConversation = false;
				}
			} else {
				ArgueTurn = ArgumentTurn.Defence;
				inNextConversation = true;
				if (ClickCount == curruntAgru.Miss_Parker.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation", ClickCount);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Miss_Parker;
						ClickCount = PlayerPrefs.GetInt ("LastConversation");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
				} else {
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Miss_Parker;
						ClickCount--;
						print (ClickCount);
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	

						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
				}
			}		
			isNext = true; 
		}
	}



	// Defence And Ms_Brewster Argument Level 4
	IEnumerator DefenceAndMs_Brewster_argumentTextShowLevel4 (Agrument curruntAgru)
	{
		if (isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Dr_Thomos.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Dr_Thomos.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Defence;

			if (ArgueTurn == ArgumentTurn.Defence) {
				LawyerTextBox.SetActive (true);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				// Change this line 
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				Mr_Peter.SetActive (false);

				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			} else if (ArgueTurn == ArgumentTurn.Ms_Brewster) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (true);
				DefinationBox.SetActive (false);		
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
				Miss_Parker.SetActive (false);
				Dr_Thomos.SetActive (false);
				VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Ms_Brewster [ClickCount].ToString ();
				if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			}

			if (ArgueTurn == ArgumentTurn.Defence) {
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);
				if (CurruntArguStep == 19 && curruntAgru.Defence [ClickCount] == "Ms Brewster, you say that you felt pressured by Dr Brown to perform certain acts but did he ever actually force you to do anything expressly against your will?") {
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (false);
				}
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					if (inNextConversation) {
						ClickCount--;
						print (ClickCount);
					}
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);

				});
				if (ClickCount == curruntAgru.Defence.Length - 1) {
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					
					});
					inNextConversation = false;
					ClickCount = -1;
					curruntAgru.Compleated = true;
					ArgueTurn = ArgumentTurn.Null;
				} else {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount--;
				}
			} else {
				PlayerPrefs.SetInt ("LastConversation1", ClickCount);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.transform.gameObject.SetActive (false);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount = PlayerPrefs.GetInt ("LastConversation1");
					ClickCount--;
					print (ClickCount);
					isObjection = false;
					isRaisedObjection = false;
					curruntAgru.Compleated = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
				});
				ArgueTurn = ArgumentTurn.Defence;
				inNextConversation = true;
			}
			isNext = true;   
		}
	}


	// Prosecution And Thomos Argument Level 4
	IEnumerator ProsecutionAndThomos_argumentTextShowLevel4 (Agrument curruntAgru)
	{
		if (isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Prosecution;

			if (ArgueTurn == ArgumentTurn.Prosecution) {
				LawyerTextBox.SetActive (true);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				// Change this line 
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);

			} else if (ArgueTurn == ArgumentTurn.Thomas_Brown) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (true);
				DefinationBox.SetActive (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);		
				SelectedLawyer.SetActive (false);
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);

				VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Thomas_Brown [ClickCount].ToString ();
				if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			}

			if (ArgueTurn == ArgumentTurn.Prosecution) {

				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);	
				if (CurruntArguStep == 12 && curruntAgru.Prosecution [ClickCount] == "When was the last time you and your wife had sex?"
				    || CurruntArguStep == 14 && curruntAgru.Prosecution [ClickCount] == "What is your opinion of exotic dancers?"
				    || CurruntArguStep == 16 && curruntAgru.Prosecution [ClickCount] == "Do you think that exotic dancers are already selling their bodies and this means that they can’t say no?") {
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (false);
				}
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					if (inNextConversation) {
						ClickCount--;
						print (ClickCount);
					}
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
				if (ClickCount == curruntAgru.Prosecution.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);

					});
					if (CurruntArguStep == 12 && curruntAgru.Prosecution [ClickCount] == "When was the last time you and your wife had sex?"
					    || CurruntArguStep == 14 && curruntAgru.Prosecution [ClickCount] == "What is your opinion of exotic dancers?") {
						BackButton1.transform.gameObject.SetActive (false);
						BackButton2.transform.gameObject.SetActive (false);
					}
					inNextConversation = false;
					ClickCount = -1;
					curruntAgru.Compleated = true;
					ArgueTurn = ArgumentTurn.Null;
				} else {
					inNextConversation = false;
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
				}
			
			} else {
				
				if (ClickCount + 1 == curruntAgru.Thomas_Brown.Length) {
					PlayerPrefs.SetInt ("LastConversation", ClickCount);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Thomas_Brown;
						ClickCount = PlayerPrefs.GetInt ("LastConversation");
						ClickCount--;
						curruntAgru.Compleated = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
					if (CurruntArguStep == 14 && curruntAgru.Thomas_Brown [ClickCount] != "Yes probably.") {
						ClickCount = -1;
						curruntAgru.Compleated = true;
						ArgueTurn = ArgumentTurn.Null;
					} else {
						ArgueTurn = ArgumentTurn.Prosecution;
						inNextConversation = true;
					}
				} else {
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Thomas_Brown;
						ClickCount--;
						print (ClickCount);
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
					ArgueTurn = ArgumentTurn.Prosecution;
					inNextConversation = true;
				}
			}
			isNext = true;  
		}
	}



	// Prosecution And Ms_Brewster Argument Level 4
	IEnumerator ProsecutionAndMs_Brewster_argumentTextShowLevel4 (Agrument curruntAgru)
	{
		if (isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Dr_Thomos.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Dr_Thomos.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Prosecution;

			if (ArgueTurn == ArgumentTurn.Prosecution) {
				LawyerTextBox.SetActive (true);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				// Change this line 
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Dr_Thomos.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);

				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [ClickCount].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);

			} else if (ArgueTurn == ArgumentTurn.Ms_Brewster) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (true);
				DefinationBox.SetActive (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);		
				SelectedLawyer.SetActive (false);
				Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
				Dr_Thomos.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);

				VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Ms_Brewster [ClickCount].ToString ();
				if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			}

			if (ArgueTurn == ArgumentTurn.Prosecution) {

				BackButton2.transform.gameObject.SetActive (false);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Prosecution;
					if (inNextConversation) {
						ClickCount--;
						print (ClickCount);
					}
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
				if (ClickCount == curruntAgru.Prosecution.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);

					});
					inNextConversation = false;
					ClickCount = -1;
					curruntAgru.Compleated = true;
					ArgueTurn = ArgumentTurn.Null;
				} else {
					inNextConversation = false;
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount--;
				}
			} else {			

				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.transform.gameObject.SetActive (false);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Ms_Brewster;
					ClickCount--;
					print (ClickCount);
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;
				});
				ArgueTurn = ArgumentTurn.Prosecution;
				inNextConversation = true;

			}

			isNext = true;
		}
	}





	// Prosecution Objection In Level 4
	IEnumerator ProsecutionObjectionLevel4_TextShow (Agrument curruntAgru)
	{
		if (!isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = "You missed an objection.";
				DefinationBox.SetActive (false);
				// Change this line 
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = "You missed an objection.";
				DefinationBox.SetActive (false);
				// Change this line 
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
			curruntAgru.Compleated = true;
			BackPressed = true;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Prosecution;

			if (ArgueTurn == ArgumentTurn.Prosecution) {
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (true);
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				// Change this line 
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}
				ObjectionControle.Instance.SetText (curruntAgru);
			
				isObjection = true;

			} else if (ArgueTurn == ArgumentTurn.Objection) {
				if (isObjection)
					isObjection = false;				
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);		
				SelectedLawyer.SetActive (false);
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Miss_Parker.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}	
				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Prosecution [0].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;

				//Objection Box
				DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
				if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
					DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
				isRaisedObjection = false;
			}

			if (ArgueTurn == ArgumentTurn.Prosecution) {	

				if (CurruntArguStep == 11 && curruntAgru.Prosecution [ClickCount] == "OBJECTION") {
					//					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						//						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (2).gameObject.SetActive (false);
					});


				} else {
					//					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Prosecution;
						//						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (2).gameObject.SetActive (false);
					});

				}
				inNextConversation = false;
				ArgueTurn = ArgumentTurn.Objection;
				ClickCount--;
			} else {
				ArgueTurn = ArgumentTurn.Null;
				ClickCount = -1;
				curruntAgru.Compleated = true;
				BackPressed = true;
			}
			isNext = true;
		}    
	}





	// Defence Objection Level 4
	IEnumerator DefenceObjectionOption_TextShowLevel4 (Agrument curruntAgru)
	{	
		if (!isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = "You missed an objection.";
				DefinationBox.SetActive (false);
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}		
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = "You missed an objection.";
				DefinationBox.SetActive (false);
				// Change this line 
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}		
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false; 
			BackPressed = true;
			curruntAgru.Compleated = true;
		} else {	
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Defence;

			if (ArgueTurn == ArgumentTurn.Defence) {
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (true);
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				// Change this line 
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}		

				ObjectionControle.Instance.SetTextForDefence (curruntAgru);
				isObjection = true;
			} else if (ArgueTurn == ArgumentTurn.Objection) {
				if (isObjection)
					isObjection = false;
				ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (true);
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				if (isParker) {
					Miss_Parker.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isBrewster) {
					Ms_Brewster.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Dr_Thomos.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isThomos) {
					Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Mr_Peter.SetActive (false);
				} else if (isPeter) {
					Mr_Peter.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
					Miss_Parker.SetActive (false);
					Ms_Brewster.SetActive (false);
					Dr_Thomos.SetActive (false);
				}
				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [0].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;

				//Objection Box
				DefinationBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.ObjectionDefination.ToString ();
				if (DefinationBox.transform.GetComponentInChildren<Text> ().text.Length > 157)
					DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					DefinationBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
				isRaisedObjection = false;
			}

			if (ArgueTurn == ArgumentTurn.Defence) {
				BackButton1.transform.gameObject.SetActive (false);
				BackButton2.transform.gameObject.SetActive (true);
				BackButton1.onClick.RemoveAllListeners ();
				BackButton1.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					ClickCount--;
					curruntAgru.Compleated = false;
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (2).gameObject.SetActive (false);
				});
				inNextConversation = false;
				ArgueTurn = ArgumentTurn.Objection;
				ClickCount--;
			} else {
				ArgueTurn = ArgumentTurn.Null;
				ClickCount = -1;
				curruntAgru.Compleated = true;
				BackPressed = true;
			}
			isNext = true; 
		}
	}




	// Defence and DrBrown Argument Level 4
	IEnumerator DrBrownAndDefence_argumentTextShowLevel4 (Agrument curruntAgru)
	{
		if (isRaisedObjection) {
			if (ArgueTurn == ArgumentTurn.Miss_Parker || ArgueTurn == ArgumentTurn.Ms_Brewster || ArgueTurn == ArgumentTurn.Thomas_Brown || ArgueTurn == ArgumentTurn.Mr_Peter) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			} else {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (false);
				InvalidObjection.SetActive (true);
				InvalidObjection.transform.GetChild (0).GetComponent<Text> ().text = curruntAgru.ObjectionDefination;
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);
			}
			ClickCount--;
			isNext = true;
			isRaisedObjection = false;
		} else {
			InvalidObjection.SetActive (false);
			if (ArgueTurn == ArgumentTurn.Null)
				ArgueTurn = ArgumentTurn.Defence;

			if (ArgueTurn == ArgumentTurn.Defence) {
				LawyerTextBox.SetActive (true);
				VictimTextBox.SetActive (false);
				DefinationBox.SetActive (false);
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (true);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (false);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);

				LawyerTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Defence [ClickCount].ToString ();
				if (LawyerTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					LawyerTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);

			} else if (ArgueTurn == ArgumentTurn.Thomas_Brown) {
				LawyerTextBox.SetActive (false);
				VictimTextBox.SetActive (true);
				DefinationBox.SetActive (false);
				SelectedLawyer.GetComponent<Lawyer> ().CharacterForgroundAndBackGround (false);
				TempAdvocates [PlayerPrefs.GetInt ("Defender")].SetActive (false);	
				Dr_Thomos.GetComponent<WitnessBoxCharacter> ().CharacterForgroundAndBackGround (true);
				Ms_Brewster.SetActive (false);
				Miss_Parker.SetActive (false);
				Mr_Peter.SetActive (false);

				VictimTextBox.transform.GetComponentInChildren<Text> ().text = curruntAgru.Thomas_Brown [ClickCount].ToString ();
				if (VictimTextBox.transform.GetComponentInChildren<Text> ().text.Length > 318)
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = true;
				else
					VictimTextBox.transform.GetComponentInChildren<ScrollRect> ().vertical = false;
				yield return new WaitForSeconds (letterPause);
			}	

			if (ArgueTurn == ArgumentTurn.Defence) {

				BackButton2.transform.gameObject.SetActive (false);
				BackButton1.transform.gameObject.SetActive (true);
				BackButton2.onClick.RemoveAllListeners ();
				BackButton2.onClick.AddListener (() => {
					ArgueTurn = ArgumentTurn.Defence;
					if (inNextConversation) {
						ClickCount--;
						print (ClickCount);
					}
					isObjection = false;
					isRaisedObjection = false;
					CaseAndLevelSelectionsBackCase ();	
					ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
					ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
					isObjection = false;

				});
				if (ClickCount == curruntAgru.Defence.Length - 1) {
					PlayerPrefs.SetInt ("LastConversation1", ClickCount);
					BackButton2.transform.gameObject.SetActive (false);
					BackButton1.transform.gameObject.SetActive (true);
					BackButton2.onClick.RemoveAllListeners ();
					BackButton2.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Defence;
						ClickCount = PlayerPrefs.GetInt ("LastConversation1");
						ClickCount--;
						curruntAgru.Compleated = false;
						isObjection = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);

					});
					inNextConversation = false;
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
				} else {
					inNextConversation = false;
					ArgueTurn = ArgumentTurn.Thomas_Brown;
					ClickCount--;
				}

			} else {	
				inNextConversation = true;
				if (ClickCount + 1 == curruntAgru.Defence.Length) {
					PlayerPrefs.SetInt ("LastConversation", ClickCount);
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Thomas_Brown;
						ClickCount = PlayerPrefs.GetInt ("LastConversation");
						ClickCount--;
						curruntAgru.Compleated = false;
						isRaisedObjection = false;
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
					ClickCount = -1;
					curruntAgru.Compleated = true;
					ArgueTurn = ArgumentTurn.Null;
				} else {
					BackButton2.transform.gameObject.SetActive (true);
					BackButton1.transform.gameObject.SetActive (false);
					BackButton1.onClick.RemoveAllListeners ();
					BackButton1.onClick.AddListener (() => {
						ArgueTurn = ArgumentTurn.Thomas_Brown;
						ClickCount--;
						print (ClickCount);
						CaseAndLevelSelectionsBackCase ();	
						ObjectionPanel.transform.GetChild (0).gameObject.SetActive (false);
						ObjectionPanel.transform.GetChild (1).gameObject.SetActive (false);
						isObjection = false;
					});
					ArgueTurn = ArgumentTurn.Defence;
				}
			}	

			isNext = true;
		}
	}

	#endregion


	public void PauseAndResume ()
	{
		isPause = !isPause ? true : false;
		GameScreenManager.Instance.PauseScreenShow ();
		if (isPause) {
			Time.timeScale = 0f;
		} else {
			Time.timeScale = 1f;
		}
	}

	public IEnumerator SaveScore (int score)
	{
		var form = new WWWForm ();

		form.AddField ("case_id", 1.ToString ());
		form.AddField ("level_id", CaseLevel.ToString ());
		form.AddField ("score", score.ToString ());

		UnityWebRequest request = UnityWebRequest.Post (DatabaseURL.PostSaveStateOfTheGame, form);
		request.SetRequestHeader ("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		yield return request.SendWebRequest ();
		var endTime = Time.time;

		if (request.error == null) {
			var response = JSON.Parse (request.downloadHandler.text);

			if (response ["statusCode"].ToString () == "200") {
				Debug.Log (response ["message"].ToString ());
			} else {
				Debug.Log (response ["message"].ToString ());
			}
		} else {
			if (request.error == "400 Bad Request") {
				var response = JSON.Parse (request.downloadHandler.text);
				string messageOnSuccess = response ["message"].ToString ();
				Debug.Log (messageOnSuccess);
			} else {
				Debug.Log ("ERROR OCCURED!!");
			}
		}
	}


	// Case And level
	public void CaseAndLevelSelections ()
	{
		BackPressed = false;
		IntroductionPanel.SetActive (false);
		Sound.PlayOneShot (Sound.clip);
		LawyerTextBox.transform.GetComponentInChildren<Text> ().transform.parent.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (-1.340015e-06f, 6.738437e-05f, 0f);
		VictimTextBox.transform.GetComponentInChildren<Text> ().transform.parent.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (-1.340015e-06f, 6.738437e-05f, 0f);
		DefinationBox.transform.GetComponentInChildren<Text> ().transform.parent.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (-1.340015e-06f, 6.738437e-05f, 0f);
		LawyerTextBox.transform.GetComponentInChildren<Text> ().text = "";
		VictimTextBox.transform.GetComponentInChildren<Text> ().text = "";
		DefinationBox.transform.GetComponentInChildren<Text> ().text = "";
		ClickCount++;
		isNext = false;
		GameScreenManager.Instance.BackgroundOnForGround (false);
		switch (Case) {
		case "Case1":
			switch (CaseLevel) {
			#region Case 1
			case 1:	
				ObjectionButton.SetActive (false);
				InvalidObjection.SetActive (false);
				if (!Case1_Level1_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level1_Argument [0]));
				} else if (!Case1_Level1_Argument [1].Compleated) {
					// Defence And Parker
					CurruntArguStep = 1;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level1_Argument [1]));
				} else if (!Case1_Level1_Argument [2].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [2]));
				} else if (!Case1_Level1_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level1_Argument [3]));
				} else if (!Case1_Level1_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [4]));
				} else if (!Case1_Level1_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level1_Argument [5]));
				} else if (!Case1_Level1_Argument [6].Compleated) {
					/// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [6]));
				} else if (!Case1_Level1_Argument [9].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 9;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level1_Argument [9]));
				} else if (!Case1_Level1_Argument [10].Compleated) {
					// DrBrown And Defence
					CurruntArguStep = 10;
					StartCoroutine (DrBrownAndDefence_argumentTextShow (Case1_Level1_Argument [10]));
				} else if (!Case1_Level1_Argument [11].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [11]));
				} else if (!Case1_Level1_Argument [12].Compleated) {
					// Prosecution And thomos
					CurruntArguStep = 12;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level1_Argument [12]));
				} else if (!Case1_Level1_Argument [13].Compleated) {
					// Objection Defence
					CurruntArguStep = 13;
					StartCoroutine (DefenceObjection_TextShow (Case1_Level1_Argument [13]));
				} else if (!Case1_Level1_Argument [14].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 14;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level1_Argument [14]));
				} else if (!Case1_Level1_Argument [15].Compleated) {
					// Objection Defence
					CurruntArguStep = 15;
					StartCoroutine (DefenceObjection_TextShow (Case1_Level1_Argument [15]));
				} else if (!Case1_Level1_Argument [16].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level1_Argument [16]));
				} else if (!Case1_Level1_Argument [17].Compleated) {
					// Prosecution And Brewster
					CurruntArguStep = 17;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level1_Argument [17]));
				} else if (!Case1_Level1_Argument [18].Compleated) {
					// Objection Defence
					CurruntArguStep = 18;
					StartCoroutine (DefenceObjection_TextShow (Case1_Level1_Argument [18]));
				} else if (!Case1_Level1_Argument [19].Compleated) {
					// Objection Defence
					CurruntArguStep = 19;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShow (Case1_Level1_Argument [19]));
				} else {
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 100);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
					BackPressed = true;
					GameScreenManager.Instance.PauseButton.SetActive (false);
				}

				break;
				#endregion

				#region Case 2
			case 2:
				ObjectionButton.SetActive (false);
				InvalidObjection.SetActive (false);
				if (!Case1_Level2_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level2_Argument [0]));
				} else if (!Case1_Level2_Argument [1].Compleated) {
					// Defence And Parker
					CurruntArguStep = 1;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level2_Argument [1]));
				} else if (!Case1_Level2_Argument [2].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [2]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [2].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level2_Argument [3]));
				} else if (!Case1_Level2_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [4]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [4].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level2_Argument [5]));
				} else if (!Case1_Level2_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [6]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [6].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [9].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 9;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level2_Argument [9]));
				} else if (!Case1_Level2_Argument [10].Compleated) {
					// DrBrown And Defence
					CurruntArguStep = 10;
					StartCoroutine (DrBrownAndDefence_argumentTextShow (Case1_Level2_Argument [10]));
				} else if (!Case1_Level2_Argument [11].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [11]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [11].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 20);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [12].Compleated) {
					// Prosecution And thomos
					CurruntArguStep = 12;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level2_Argument [12]));
				} else if (!Case1_Level2_Argument [13].Compleated) {
					// Objection Defence
					CurruntArguStep = 13;
					StartCoroutine (DefenceObjectionOption_TextShow (Case1_Level2_Argument [13]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [13].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [14].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 14;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level2_Argument [14]));
				} else if (!Case1_Level2_Argument [15].Compleated) {
					// Objection Defence
					CurruntArguStep = 15;
					StartCoroutine (DefenceObjectionOption_TextShow (Case1_Level2_Argument [15]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [15].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [16].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level2_Argument [16]));
				} else if (!Case1_Level2_Argument [17].Compleated) {
					// Prosecution And Brewster
					CurruntArguStep = 17;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level2_Argument [17]));
				} else if (!Case1_Level2_Argument [18].Compleated) {
					// Objection Defence
					CurruntArguStep = 18;
					StartCoroutine (DefenceObjectionOption_TextShow (Case1_Level2_Argument [18]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [18].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [19].Compleated) {
					// Objection Defence
					CurruntArguStep = 19;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShow (Case1_Level2_Argument [19]));
				} else {
					
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
					BackPressed = true;
					GameScreenManager.Instance.PauseButton.SetActive (false);
				}
				break;
				#endregion

				#region Case 3
			case 3:
				ObjectionButton.SetActive (false);
				InvalidObjection.SetActive (false);
				if (!Case1_Level3_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level3_Argument [0]));
				} else if (!Case1_Level3_Argument [1].Compleated) {
					// Defence Objection
					CurruntArguStep = 1;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [1]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [1].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [2].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionAndParkerFirst_argumentTextShow (Case1_Level3_Argument [2]));
				} else if (!Case1_Level3_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level3_Argument [3]));
				} else if (!Case1_Level3_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [4]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [4].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level3_Argument [5]));
				} else if (!Case1_Level3_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [6]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [6].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [7].Compleated) {
					// Defence And Parker
					CurruntArguStep = 7;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level3_Argument [7]));
				} else if (!Case1_Level3_Argument [8].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 8;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [8]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [8].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [11].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level3_Argument [11]));
				} else if (!Case1_Level3_Argument [12].Compleated) {
					// Defence And thomos
					CurruntArguStep = 12;
					StartCoroutine (DrBrownAndDefence_argumentTextShow (Case1_Level3_Argument [12]));
				} else if (!Case1_Level3_Argument [13].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 13;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [13]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [13].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [14].Compleated) {
					// Defence And thomos
					CurruntArguStep = 14;
					StartCoroutine (FirstDrBrownAndDefence_argumentTextShow (Case1_Level3_Argument [14]));
				} else if (!Case1_Level3_Argument [15].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 15;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [15]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [15].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [16].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level3_Argument [16]));
				} else if (!Case1_Level3_Argument [17].Compleated) {
					// Defence Objection
					CurruntArguStep = 17;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [17]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [17].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [18].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 18;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level3_Argument [18]));

				} else if (!Case1_Level3_Argument [19].Compleated) {
					// Defence Objection
					CurruntArguStep = 19;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [19]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [19].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [20].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 20;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level3_Argument [20]));

				} else if (!Case1_Level3_Argument [21].Compleated) {
					// Prosecution And Ms_Brewster
					CurruntArguStep = 21;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level3_Argument [21]));

				} else if (!Case1_Level3_Argument [22].Compleated) {
					// Defence Objection
					CurruntArguStep = 22;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [22]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [22].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [23].Compleated) {
					// Prosecution And Ms_Brewster
					CurruntArguStep = 23;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level3_Argument [23]));
				} else if (!Case1_Level3_Argument [24].Compleated) {
					// Defence Objection
					CurruntArguStep = 24;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [24]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [24].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 12);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [25].Compleated) {
					// Defence And Ms_Brewster
					CurruntArguStep = 25;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShow (Case1_Level3_Argument [25]));
				} else {
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
					BackPressed = true;
					GameScreenManager.Instance.PauseButton.SetActive (false);
				}
				break;
				#endregion

				#region Case 4
			case 4:
				ObjectionButton.SetActive (true);
				if (!Case1_Level4_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [0]));

				} else if (!Case1_Level4_Argument [1].Compleated) {
					// Defence And Parker
					CurruntArguStep = 1;
					StartCoroutine (DefenceAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [1]));
				} else if (!Case1_Level4_Argument [2].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [2]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [2].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [3]));
				} else if (!Case1_Level4_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [4]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [4].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);

						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [5]));
				} else if (!Case1_Level4_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [6]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [6].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [9].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 9;
					StartCoroutine (ProsecutionAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [9]));

				} else if (!Case1_Level4_Argument [10].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 10;
					StartCoroutine (DrBrownAndDefence_argumentTextShowLevel4 (Case1_Level4_Argument [10]));
				} else if (!Case1_Level4_Argument [11].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [11]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [11].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [12].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 12;
					StartCoroutine (ProsecutionAndThomos_argumentTextShowLevel4 (Case1_Level4_Argument [12]));
				} else if (!Case1_Level4_Argument [13].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 13;
					StartCoroutine (DefenceObjectionOption_TextShowLevel4 (Case1_Level4_Argument [13]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [13].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [14].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 14;
					StartCoroutine (ProsecutionAndThomos_argumentTextShowLevel4 (Case1_Level4_Argument [14]));
				} else if (!Case1_Level4_Argument [15].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 15;
					StartCoroutine (DefenceObjectionOption_TextShowLevel4 (Case1_Level4_Argument [15]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [15].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [16].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShowLevel4 (Case1_Level4_Argument [16]));
				} else if (!Case1_Level4_Argument [17].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 17;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShowLevel4 (Case1_Level4_Argument [17]));
				} else if (!Case1_Level4_Argument [18].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 18;
					StartCoroutine (DefenceObjectionOption_TextShowLevel4 (Case1_Level4_Argument [18]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [18].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [19].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 19;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShowLevel4 (Case1_Level4_Argument [19]));
				} else {
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
					BackPressed = true;
					GameScreenManager.Instance.PauseButton.SetActive (false);
				}
				break;

				#endregion
			}

			break;
		}
	}


	// Case And level
	public void CaseAndLevelSelectionsBackCase ()
	{
		BackPressed = true;
		IntroductionPanel.SetActive (false);
		Sound.PlayOneShot (Sound.clip);
		LawyerTextBox.transform.GetComponentInChildren<Text> ().transform.parent.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (-1.340015e-06f, 6.738437e-05f, 0f);
		VictimTextBox.transform.GetComponentInChildren<Text> ().transform.parent.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (-1.340015e-06f, 6.738437e-05f, 0f);
		DefinationBox.transform.GetComponentInChildren<Text> ().transform.parent.gameObject.GetComponent<RectTransform> ().localPosition = new Vector3 (-1.340015e-06f, 6.738437e-05f, 0f);
		LawyerTextBox.transform.GetComponentInChildren<Text> ().text = "";
		VictimTextBox.transform.GetComponentInChildren<Text> ().text = "";
		DefinationBox.transform.GetComponentInChildren<Text> ().text = "";
		ClickCount++;
		isNext = false;
		GameScreenManager.Instance.BackgroundOnForGround (false);
		switch (Case) {
		case "Case1":
			switch (CaseLevel) {
			case 1:	
				ObjectionButton.SetActive (false);
				InvalidObjection.SetActive (false);
				if (!Case1_Level1_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level1_Argument [0]));
				} else if (!Case1_Level1_Argument [1].Compleated) {
					// Defence And Parker
					CurruntArguStep = 1;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level1_Argument [1]));
				} else if (!Case1_Level1_Argument [2].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [2]));
				} else if (!Case1_Level1_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level1_Argument [3]));
				} else if (!Case1_Level1_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [4]));
				} else if (!Case1_Level1_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level1_Argument [5]));
				} else if (!Case1_Level1_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [6]));
				} else if (!Case1_Level1_Argument [9].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 9;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level1_Argument [9]));
				} else if (!Case1_Level1_Argument [10].Compleated) {
					// DrBrown And Defence
					CurruntArguStep = 10;
					StartCoroutine (DrBrownAndDefence_argumentTextShow (Case1_Level1_Argument [10]));
				} else if (!Case1_Level1_Argument [11].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionObjection_TextShow (Case1_Level1_Argument [11]));
				} else if (!Case1_Level1_Argument [12].Compleated) {
					// Prosecution And thomos
					CurruntArguStep = 12;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level1_Argument [12]));
				} else if (!Case1_Level1_Argument [13].Compleated) {
					// Objection Defence
					CurruntArguStep = 13;
					StartCoroutine (DefenceObjection_TextShow (Case1_Level1_Argument [13]));
				} else if (!Case1_Level1_Argument [14].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 14;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level1_Argument [14]));
				} else if (!Case1_Level1_Argument [15].Compleated) {
					// Objection Defence
					CurruntArguStep = 15;
					StartCoroutine (DefenceObjection_TextShow (Case1_Level1_Argument [15]));
				} else if (!Case1_Level1_Argument [16].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level1_Argument [16]));
				} else if (!Case1_Level1_Argument [17].Compleated) {
					// Prosecution And Brewster
					CurruntArguStep = 17;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level1_Argument [17]));
				} else if (!Case1_Level1_Argument [18].Compleated) {
					// Objection Defence
					CurruntArguStep = 18;
					StartCoroutine (DefenceObjection_TextShow (Case1_Level1_Argument [18]));
				} else if (!Case1_Level1_Argument [19].Compleated) {
					// Objection Defence
					CurruntArguStep = 19;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShow (Case1_Level1_Argument [19]));
				} else {
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 100);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
				}

				break;
			case 2:
				ObjectionButton.SetActive (false);
				InvalidObjection.SetActive (false);
				if (!Case1_Level2_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level2_Argument [0]));
				} else if (!Case1_Level2_Argument [1].Compleated) {
					// Defence And Parker
					CurruntArguStep = 1;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level2_Argument [1]));
				} else if (!Case1_Level2_Argument [2].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [2]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [2].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level2_Argument [3]));
				} else if (!Case1_Level2_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [4]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [4].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level2_Argument [5]));
				} else if (!Case1_Level2_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [6]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [6].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [9].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 9;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level2_Argument [9]));
				} else if (!Case1_Level2_Argument [10].Compleated) {
					// DrBrown And Defence
					CurruntArguStep = 10;
					StartCoroutine (DrBrownAndDefence_argumentTextShow (Case1_Level2_Argument [10]));
				} else if (!Case1_Level2_Argument [11].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionObjectionOption_TextShow (Case1_Level2_Argument [11]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [11].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 20);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [12].Compleated) {
					// Prosecution And thomos
					CurruntArguStep = 12;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level2_Argument [12]));
				} else if (!Case1_Level2_Argument [13].Compleated) {
					// Objection Defence
					CurruntArguStep = 13;
					StartCoroutine (DefenceObjectionOption_TextShow (Case1_Level2_Argument [13]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [13].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [14].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 14;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level2_Argument [14]));
				} else if (!Case1_Level2_Argument [15].Compleated) {
					// Objection Defence
					CurruntArguStep = 15;
					StartCoroutine (DefenceObjectionOption_TextShow (Case1_Level2_Argument [15]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [15].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [16].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level2_Argument [16]));
				} else if (!Case1_Level2_Argument [17].Compleated) {
					// Prosecution And Brewster
					CurruntArguStep = 17;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level2_Argument [17]));
				} else if (!Case1_Level2_Argument [18].Compleated) {
					// Objection Defence
					CurruntArguStep = 18;
					StartCoroutine (DefenceObjectionOption_TextShow (Case1_Level2_Argument [18]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level2_Argument [18].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level2_Argument [19].Compleated) {
					// Objection Defence
					CurruntArguStep = 19;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShow (Case1_Level2_Argument [19]));
				} else {

					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
				}
				break;
			case 3:
				ObjectionButton.SetActive (false);
				InvalidObjection.SetActive (false);
				if (!Case1_Level3_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level3_Argument [0]));
				} else if (!Case1_Level3_Argument [1].Compleated) {
					// Defence Objection
					CurruntArguStep = 1;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [1]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [1].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [2].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionAndParkerFirst_argumentTextShow (Case1_Level3_Argument [2]));
				} else if (!Case1_Level3_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level3_Argument [3]));
				} else if (!Case1_Level3_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [4]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [4].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level3_Argument [5]));
				} else if (!Case1_Level3_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [6]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [6].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [7].Compleated) {
					// Defence And Parker
					CurruntArguStep = 7;
					StartCoroutine (DefenceAndParker_argumentTextShow (Case1_Level3_Argument [7]));
				} else if (!Case1_Level3_Argument [8].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 8;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [8]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [8].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [11].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionAndParker_argumentTextShow (Case1_Level3_Argument [11]));
				} else if (!Case1_Level3_Argument [12].Compleated) {
					// Defence And thomos
					CurruntArguStep = 12;
					StartCoroutine (DrBrownAndDefence_argumentTextShow (Case1_Level3_Argument [12]));
				} else if (!Case1_Level3_Argument [13].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 13;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [13]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [13].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [14].Compleated) {
					// Defence And thomos
					CurruntArguStep = 14;
					StartCoroutine (FirstDrBrownAndDefence_argumentTextShow (Case1_Level3_Argument [14]));
				} else if (!Case1_Level3_Argument [15].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 15;
					StartCoroutine (JudgeDecision_ProsecutionObjection_TextShow (Case1_Level3_Argument [15]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [15].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [16].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level3_Argument [16]));
				} else if (!Case1_Level3_Argument [17].Compleated) {
					// Defence Objection
					CurruntArguStep = 17;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [17]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [17].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [18].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 18;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level3_Argument [18]));

				} else if (!Case1_Level3_Argument [19].Compleated) {
					// Defence Objection
					CurruntArguStep = 19;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [19]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [19].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [20].Compleated) {
					// Prosecution And Thomos
					CurruntArguStep = 20;
					StartCoroutine (ProsecutionAndThomos_argumentTextShow (Case1_Level3_Argument [20]));

				} else if (!Case1_Level3_Argument [21].Compleated) {
					// Prosecution And Ms_Brewster
					CurruntArguStep = 21;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level3_Argument [21]));

				} else if (!Case1_Level3_Argument [22].Compleated) {
					// Defence Objection
					CurruntArguStep = 22;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [22]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [22].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 8);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [23].Compleated) {
					// Prosecution And Ms_Brewster
					CurruntArguStep = 23;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShow (Case1_Level3_Argument [23]));
				} else if (!Case1_Level3_Argument [24].Compleated) {
					// Defence Objection
					CurruntArguStep = 24;
					StartCoroutine (JudgeDecision_DefenceObjectionOption_TextShow (Case1_Level3_Argument [24]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level3_Argument [24].Judge [0] == ObjectionControle.Instance.JudgDecision) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 12);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));
						}
					}
				} else if (!Case1_Level3_Argument [25].Compleated) {
					// Defence And Ms_Brewster
					CurruntArguStep = 25;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShow (Case1_Level3_Argument [25]));
				} else {
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
				}
				break;
			case 4:
				ObjectionButton.SetActive (true);
				if (!Case1_Level4_Argument [0].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 0;
					StartCoroutine (ProsecutionAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [0]));

				} else if (!Case1_Level4_Argument [1].Compleated) {
					// Defence And Parker
					CurruntArguStep = 1;
					StartCoroutine (DefenceAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [1]));
				} else if (!Case1_Level4_Argument [2].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 2;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [2]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [2].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [3].Compleated) {
					// Defence And Parker
					CurruntArguStep = 3;
					StartCoroutine (DefenceAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [3]));
				} else if (!Case1_Level4_Argument [4].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 4;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [4]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [4].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);

						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [5].Compleated) {
					// Defence And Parker
					CurruntArguStep = 5;
					StartCoroutine (DefenceAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [5]));
				} else if (!Case1_Level4_Argument [6].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 6;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [6]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [6].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [9].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 9;
					StartCoroutine (ProsecutionAndParker_argumentTextShowLevel4 (Case1_Level4_Argument [9]));

				} else if (!Case1_Level4_Argument [10].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 10;
					StartCoroutine (DrBrownAndDefence_argumentTextShowLevel4 (Case1_Level4_Argument [10]));
				} else if (!Case1_Level4_Argument [11].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 11;
					StartCoroutine (ProsecutionObjectionLevel4_TextShow (Case1_Level4_Argument [11]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [11].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [12].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 12;
					StartCoroutine (ProsecutionAndThomos_argumentTextShowLevel4 (Case1_Level4_Argument [12]));
				} else if (!Case1_Level4_Argument [13].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 13;
					StartCoroutine (DefenceObjectionOption_TextShowLevel4 (Case1_Level4_Argument [13]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [13].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 10);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [14].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 14;
					StartCoroutine (ProsecutionAndThomos_argumentTextShowLevel4 (Case1_Level4_Argument [14]));
				} else if (!Case1_Level4_Argument [15].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 15;
					StartCoroutine (DefenceObjectionOption_TextShowLevel4 (Case1_Level4_Argument [15]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [15].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [16].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 16;
					StartCoroutine (ProsecutionAndThomos_argumentTextShowLevel4 (Case1_Level4_Argument [16]));
				} else if (!Case1_Level4_Argument [17].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 17;
					StartCoroutine (ProsecutionAndMs_Brewster_argumentTextShowLevel4 (Case1_Level4_Argument [17]));
				} else if (!Case1_Level4_Argument [18].Compleated) {
					// Prosecution Objection
					CurruntArguStep = 18;
					StartCoroutine (DefenceObjectionOption_TextShowLevel4 (Case1_Level4_Argument [18]));
					if (ArgueTurn == ArgumentTurn.Objection) {
						if (Case1_Level4_Argument [18].CorrectAns == ObjectionControle.Instance.Ans) {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
						} else {
							PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score));

						}
					}
				} else if (!Case1_Level4_Argument [19].Compleated) {
					// Prosecution And Parker
					CurruntArguStep = 19;
					StartCoroutine (DefenceAndMs_Brewster_argumentTextShowLevel4 (Case1_Level4_Argument [19]));
				} else {
					PlayerPrefs.SetInt (PrefPaths.Score, PlayerPrefs.GetInt (PrefPaths.Score) + 15);
					if (PlayerPrefs.GetInt (PrefPaths.Score) > PlayerPrefs.GetInt ("CurruntLevelSore")) {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt (PrefPaths.Score)));
					} else {
						StartCoroutine (SaveScore (PlayerPrefs.GetInt ("CurruntLevelSore")));
					}
					GameScreenManager.Instance.GameWinLoseStatus ("Level " + CaseLevel, PlayerPrefs.GetInt (PrefPaths.Score));
				}
				break;
			}

			break;
		}
	}


	public void RaiseObjection ()
	{
		if (isNext) {
			isRaisedObjection = true;
			CaseAndLevelSelections ();
		}
	}

	public void MusicControle ()
	{
		this.GetComponent<AudioSource> ().volume = MusicSlider.value;
		PlayerPrefs.SetFloat ("MusicSlider", MusicSlider.value);
	}

	public void SoundsControle ()
	{
		Sound.volume = SoundSlider.value;
		PlayerPrefs.SetFloat ("SoundSlider", SoundSlider.value);

	}

	void ToggalAndSoundState ()
	{
		if (PlayerPrefs.GetInt ("VibrationState") == 1)
			isVibration = true;
		else
			isVibration = false;
		if (PlayerPrefs.GetInt ("TutorialState") == 1)
			isTutorial = true;
		else
			isTutorial = false;
		VibrationControle ();
		TutorialControle ();

		MusicSlider.value = PlayerPrefs.GetFloat ("MusicSlider");
		SoundSlider.value = PlayerPrefs.GetFloat ("SoundSlider");
		SoundsControle ();
		MusicControle ();
	}

	public void VibrationState ()
	{
		isVibration = !isVibration ? true : false;
		if (isVibration)
			PlayerPrefs.SetInt ("VibrationState", 1);
		else
			PlayerPrefs.SetInt ("VibrationState", 0);

		VibrationControle ();
		if (GameManager.Instance.isVibration)
			Handheld.Vibrate ();
	}

	public void TutorialState ()
	{
		isTutorial = !isTutorial ? true : false;
		if (isTutorial)
			PlayerPrefs.SetInt ("TutorialState", 1);
		else
			PlayerPrefs.SetInt ("TutorialState", 0);
		TutorialControle ();
	}


	void VibrationControle ()
	{
		
		var Pos = VibrationToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition;
		if (isVibration)
			VibrationToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (46f, 0f, 0f);
		else
			VibrationToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (-46f, 0f, 0f);
		
	}

	void TutorialControle ()
	{
		
		var Pos = TutorialToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition;
		if (isTutorial)
			TutorialToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (49f, 0f, 0f);
		else
			TutorialToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (-49f, 0f, 0f);

	}

	public void SetIntroductionForLevel ()
	{
		IntroductionPanel.SetActive (true);
		if (CaseLevel == 1) {
			IntroductionPanel.transform.GetChild (0).GetComponent<Text> ().text = LevelIntroduction [0].Title;
			IntroductionPanel.transform.GetChild (1).GetComponent<Text> ().text = LevelIntroduction [0].Discription;
		} else if (CaseLevel == 2) {
			IntroductionPanel.transform.GetChild (0).GetComponent<Text> ().text = LevelIntroduction [1].Title;
			IntroductionPanel.transform.GetChild (1).GetComponent<Text> ().text = LevelIntroduction [1].Discription;
		} else if (CaseLevel == 3) {
			IntroductionPanel.transform.GetChild (0).GetComponent<Text> ().text = LevelIntroduction [2].Title;
			IntroductionPanel.transform.GetChild (1).GetComponent<Text> ().text = LevelIntroduction [2].Discription;
		} else if (CaseLevel == 4) {
			IntroductionPanel.transform.GetChild (0).GetComponent<Text> ().text = LevelIntroduction [3].Title;
			IntroductionPanel.transform.GetChild (1).GetComponent<Text> ().text = LevelIntroduction [3].Discription;
		}

	
	}


}


[Serializable]
public class Agrument
{
	public string[] Miss_Parker;
	public string[] Thomas_Brown;
	public string[] Mr_Peter;
	public string[] Ms_Brewster;
	public string[] Prosecution;
	public string[] Defence;
	public string[] Judge;
	public string ObjectionDefination;
	public string[] ObjectionType;
	public int CorrectAns;
	public bool Compleated;

}

[Serializable]
public class LevelIntro
{
	public String Title;
	public String Discription;
}

public enum ArgumentTurn
{
	Null,
	Miss_Parker,
	Thomas_Brown,
	Mr_Peter,
	Ms_Brewster,
	Prosecution,
	Defence,
	Judge,
	Objection}
;


