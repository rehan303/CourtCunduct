using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour 
{
    public static ScreenManager instance;
	public Camera Cam;
	[Header("Sounds Atributes")]
	public Slider MusicSlider;
	public Slider SoundSlider;
	public AudioSource Sound;
	public GameObject VibrationToggal, TutorialToggal;
	public bool isVibration, isTutorial;

    [SerializeField]
    private GameObject MainBackground_Panel;
    [SerializeField]
    private GameObject Registration_Panel;

    [SerializeField]
    private GameObject HomeScreen_Panel;
	[SerializeField]
	private GameObject SettingsPanel;

	public GameObject SignIn_Panel;
	public GameObject SignUp_Panel;

    [SerializeField]
    private GameObject ForgotPassword_Panel;
	[SerializeField]
	private GameObject ProfileScreen_Panel;

	[SerializeField]
	private GameObject CaseSelectionPanel;

	[SerializeField]
	private GameObject CharacterSelectionPanel;

	[SerializeField]
	private GameObject LevelSelectionPanel;

	[SerializeField]
	private GameObject TargetPosition_Left;
	[SerializeField]
	private GameObject TargetPosition_Right;
	[SerializeField]
	private GameObject TargetPosition_Center;
	[SerializeField]
	private GameObject TargetPosition_Up;
	[SerializeField]
	private GameObject TargetPosition_Down;

	public RectTransform LevelSelectionBack, CaseSelectionBack, SettingBack, ProfileBack;

	// Use this for initialization
	void Awake () 
    {
//		PlayerPrefs.SetString (PrefPaths.RegistrationSceneBackFromGamePlay, "NO");
        if(instance == null)
        {
            instance = this;
        }
//		if(PlayerPrefs.GetString (PrefPaths.RegistrationSceneBackFromGamePlay) == "YES")
//		{
//			Registration_Navigation ("OpenHomeScreen");
//		}
//		else
//		{
//			Registration_Navigation ("Login");
//		}



	}
	
	// Update is called once per frame
	void Start () 
	{
		ToggalAndSoundState ();
		
		if(PlayerPrefs.HasKey (PrefPaths.EmailId) && PlayerPrefs.HasKey (PrefPaths.password))
		{
			StartCoroutine(Registration.instance.SignIn_Registration(PlayerPrefs.GetString (PrefPaths.EmailId),PlayerPrefs.GetString (PrefPaths.password)));
//			Registration_Navigation ("OpenHomeScreen");
		}
		else
		{
			iTween.MoveTo (SignIn_Panel, iTween.Hash ("position",TargetPosition_Center.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
		}
	}

	void Update()
	{
		if (Cam.aspect >= 1.777778f) {
			SettingsPanel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			SignIn_Panel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			SignUp_Panel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			ForgotPassword_Panel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			ProfileScreen_Panel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			CaseSelectionPanel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			CharacterSelectionPanel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);
			LevelSelectionPanel.transform.localScale = new Vector3 (0.9f, 0.9f, 0.9f);

			LevelSelectionBack.anchoredPosition = new Vector2(-227.8f,48.63379f); 
			CaseSelectionBack.anchoredPosition = new Vector2(-191f,-5.238281f); 
			ProfileBack.anchoredPosition= new Vector2 (-191, -49.7f) ;
			SettingBack.anchoredPosition = new Vector2(-78f,-112f); 
		}
		else
		{
			SettingsPanel.transform.localScale = Vector3.one;
			SignIn_Panel.transform.localScale = Vector3.one;
			SignUp_Panel.transform.localScale = Vector3.one;
			ForgotPassword_Panel.transform.localScale = Vector3.one;
			ProfileScreen_Panel.transform.localScale = Vector3.one;
			CaseSelectionPanel.transform.localScale = Vector3.one;
			CharacterSelectionPanel.transform.localScale =Vector3.one;
			LevelSelectionPanel.transform.localScale = Vector3.one;

			LevelSelectionBack.anchoredPosition = new Vector2(-106.9016f,43.50781f); 
			CaseSelectionBack.anchoredPosition = new Vector2(-70.1f,-5.238281f); 
			ProfileBack.anchoredPosition= new Vector2 (-68.52747f, -49.7f) ;
			SettingBack.anchoredPosition = new Vector2(-78f,-112f); 
		}

	}

    // ScreenNavigation
    public void Registration_Navigation(string buttonPressed)
    {
        switch (buttonPressed)
        {
            case "SignUp":
//                SignIn_Panel.SetActive(false);
//                SignUp_Panel.SetActive(true);
				iTween.MoveTo (SignIn_Panel, iTween.Hash ("position",TargetPosition_Left.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
				iTween.MoveTo (SignUp_Panel, iTween.Hash ("position",TargetPosition_Center.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
                Registration.instance.ClearAllFieldsSignIn();
				ProfileScreen_Panel.SetActive(false);
				HomeScreen_Panel.SetActive (false);
				MainBackground_Panel.SetActive(true);
				ProfileScreen_Panel.transform.GetChild (0).gameObject.SetActive (false);
				ProfileScreen_Panel.transform.GetChild (1).gameObject.SetActive (false);
				Registration_Panel.SetActive(true);
                break;
            case "Login":
//                SignUp_Panel.SetActive(false);
//                SignIn_Panel.SetActive(true);
				iTween.MoveTo (SignUp_Panel, iTween.Hash ("position",TargetPosition_Right.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
				iTween.MoveTo (SignIn_Panel, iTween.Hash ("position",TargetPosition_Center.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));

                Registration.instance.ClearAllFieldsSignUp();
				ProfileScreen_Panel.SetActive(false);
				HomeScreen_Panel.SetActive (false);
				MainBackground_Panel.SetActive(true);
				ProfileScreen_Panel.transform.GetChild (0).gameObject.SetActive (false);
				ProfileScreen_Panel.transform.GetChild (1).gameObject.SetActive (false);
				Registration_Panel.SetActive(true);
                break;
            case "ForgotPassword":
//                SignIn_Panel.SetActive(false);
//                ForgotPassword_Panel.SetActive(true);
				iTween.MoveTo (SignIn_Panel, iTween.Hash ("position",TargetPosition_Down.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
				iTween.MoveTo (ForgotPassword_Panel, iTween.Hash ("position",TargetPosition_Center.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));

                Registration.instance.ClearAllFieldsSignIn();
                break;
            case "CrossForgot":
//                ForgotPassword_Panel.SetActive(false);
//                SignIn_Panel.SetActive(true);
				iTween.MoveTo (ForgotPassword_Panel, iTween.Hash ("position",TargetPosition_Up.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
				iTween.MoveTo (SignIn_Panel, iTween.Hash ("position",TargetPosition_Center.transform.position,"time", 0.4f, "easeType", iTween.EaseType.linear));
                Registration.instance.ClearAllFieldsForgotPassword();
                break;
            case "OpenHomeScreen":
                SignIn_Panel.SetActive(false);
                Registration_Panel.SetActive(false);
                MainBackground_Panel.SetActive(false);
                HomeScreen_Panel.SetActive(true);
				ProfileScreen_Panel.SetActive (false);
				ProfileScreen_Panel.transform.GetChild (0).gameObject.SetActive (false);
				ProfileScreen_Panel.transform.GetChild (1).gameObject.SetActive (false);
				StartCoroutine (HomeScreenManager.instance.AnimationCourtConduct ());
                break;
		case "HomeScreenBack":
//				StartCoroutine (Registration.instance.LogoutFromTheGame ());
				SettingsPanel.SetActive (false);
				HomeScreen_Panel.SetActive (false);
				Registration_Panel.SetActive (true);
				SignIn_Panel.SetActive (true);
				iTween.MoveTo (SignIn_Panel, iTween.Hash ("position", TargetPosition_Center.transform.position, "time", 0.4f, "easeType", iTween.EaseType.linear));
				MainBackground_Panel.SetActive (true);
				ProfileScreen_Panel.SetActive (false);
				Registration.instance.ClearAllFieldsSignIn ();
				HomeScreenManager.instance.ResetPositionForAnimations ();
				PlayerPrefs.DeleteAll ();
				RegistrationWithFacebook.Instance.TempLogOutLogOut ();
//				StartCoroutine (HomeScreenManager.instance.AnimationCourtConduct ());
                break;
			case "ProfileScreen":
//				SignUp_Panel.SetActive(false);
//				SignIn_Panel.SetActive (false);
				ProfileScreen_Panel.SetActive(true);
				HomeScreen_Panel.SetActive (false);
				MainBackground_Panel.SetActive(true);
				ProfileScreen_Panel.transform.GetChild (0).gameObject.SetActive (false);
				ProfileScreen_Panel.transform.GetChild (1).gameObject.SetActive (true);
				HomeScreenManager.instance.ResetPositionForAnimations ();
				break;
			case "ProfileScreenEdit":
				SignUp_Panel.SetActive(false);
				SignIn_Panel.SetActive (false);
				ProfileScreen_Panel.SetActive(true);
				HomeScreen_Panel.SetActive (false);
				ProfileScreen_Panel.transform.GetChild (0).gameObject.SetActive (true);
				ProfileScreen_Panel.transform.GetChild (1).gameObject.SetActive (false);
				break;
			case "HomeScreenPlayButton":
				HomeScreen_Panel.SetActive(false);
				MainBackground_Panel.SetActive(true);
				CaseSelectionPanel.SetActive (true);
				HomeScreenManager.instance.ResetPositionForAnimations ();
				break;
			case "CaseSelectionButton":
				CaseSelectionPanel.SetActive (false);
				MainBackground_Panel.SetActive(false);
				CharacterSelectionPanel.SetActive (true);
				break;
			case "BackToHomeScreenFromCaseSelection":
				HomeScreen_Panel.SetActive (true);
				MainBackground_Panel.SetActive (false);
				CaseSelectionPanel.SetActive (false);
				StartCoroutine (HomeScreenManager.instance.AnimationCourtConduct ());
				break;
			case "BackToCaseSelectionFromCharacterSelection":
				CaseSelectionPanel.SetActive (true);
				MainBackground_Panel.SetActive(true);
				CharacterSelectionPanel.SetActive (false);
				LevelSelectionPanel.SetActive (false);
				CaseSelectionManager.instance.Case1.Clear ();
				break;
			case "OpenSettingsScreen":
				SettingsPanel.SetActive (true);
				break;
			case "BackToHomeScreenFromSettings":
				SettingsPanel.SetActive (false);
				break;

        }
    }
	public void SelectedCharacter(string _selectedCharacter)
	{
		PlayerPrefs.SetString("SelectedProsecutor", _selectedCharacter);
		CharacterSelectionPanel.SetActive (false);
		MainBackground_Panel.SetActive(true);
		LevelSelectionPanel.SetActive (true);
		StartCoroutine (CaseSelectionManager.instance.GetSaveStateOfTheGame ());
	}

	public void BackToCharacterSelectionscreen()
	{
		CharacterSelectionPanel.SetActive (true);
		MainBackground_Panel.SetActive(false);
		LevelSelectionPanel.SetActive (false);
		CaseSelectionManager.instance.Case1.Clear ();
	}

	public void CaseSelection (string Case)
	{
		if(Case == "Case1")
		{
			PlayerPrefs.SetString ("Case",Case);
//			Registration_Navigation ("CaseSelectionButton");
			CharacterSelectionPanel.SetActive (false);
			CaseSelectionPanel.SetActive (false);
			MainBackground_Panel.SetActive(true);
			LevelSelectionPanel.SetActive (true);
			StartCoroutine (CaseSelectionManager.instance.GetSaveStateOfTheGame ());
		}
		else
		{
			PopUpManager.instance.ShowPopUp ("COMING SOON", "Case will be coming in next version");
		}
	}

	public void LevelSelection(int levelno)
	{
		if(levelno == 1 || levelno == 2 || levelno == 3 || levelno == 4)
		{
			if(CaseSelectionManager.instance.LevelsObject[levelno-1].transform.GetChild (1).gameObject.activeInHierarchy) {
				PlayerPrefs.SetInt ("SelectedLevel", levelno);
				UnityEngine.SceneManagement.SceneManager.LoadScene ("01_MainGamePlayScene");
				PlayerPrefs.SetInt ("CurruntLevelSore", CaseSelectionManager.instance.LevelsObject[levelno-1].GetComponent<LevelSelection> ().ThisLevel.Scores);

			}
		}
//		else
//		{
////			PopUpManager.instance.ShowPopUp ("Level " + levelno + " is locked", "Please clear all previous levels first");
//		}
	}

	void ToggalAndSoundState()
	{
		if(!PlayerPrefs.HasKey ("MusicSlider"))
		{
			PlayerPrefs.SetFloat ("MusicSlider",MusicSlider.value);
			PlayerPrefs.SetFloat ("SoundSlider", SoundSlider.value);
			PlayerPrefs.SetInt ("VibrationState", 1);
		} else {

			if (PlayerPrefs.GetInt ("VibrationState") == 1)
				isVibration = true;
			else
				isVibration = false;
			if (PlayerPrefs.GetInt ("TutorialState") == 1)
				isTutorial = true;
			else
				isTutorial = false;			
		}
		VibrationControle ();
		TutorialControle ();

		MusicSlider.value = PlayerPrefs.GetFloat ("MusicSlider");
		SoundSlider.value = PlayerPrefs.GetFloat ("SoundSlider");
		SoundsControle ();
		MusicControle ();
	}
	public void MusicControle()
	{
		this.GetComponent<AudioSource> ().volume = MusicSlider.value;
		PlayerPrefs.SetFloat ("MusicSlider", MusicSlider.value);
	}

	public void SoundsControle()
	{
		Sound.volume = SoundSlider.value;
		PlayerPrefs.SetFloat ("SoundSlider", SoundSlider.value);

	}


	public void VibrationState()
	{
		isVibration = !isVibration ? true : false;
		if (isVibration)
			PlayerPrefs.SetInt ("VibrationState", 1);
		else
			PlayerPrefs.SetInt ("VibrationState", 0);

		VibrationControle ();

		if(isVibration)
			Handheld.Vibrate();
	}
	public void TutorialState()
	{
		isTutorial = !isTutorial ? true : false;
		if (isTutorial)
			PlayerPrefs.SetInt ("TutorialState", 1);
		else
			PlayerPrefs.SetInt ("TutorialState", 0);
		TutorialControle ();
	}


	void VibrationControle()
	{

		var Pos = VibrationToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition;
		if(isVibration)			
			VibrationToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (46f, 0f, 0f);
		else			
			VibrationToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (-46f, 0f, 0f);

	}

	void TutorialControle()
	{

		var Pos = TutorialToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition;
		if(isTutorial)			
			TutorialToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (49f, 0f, 0f);
		else			
			TutorialToggal.transform.GetChild (0).GetComponent<RectTransform> ().localPosition = new Vector3 (-49f, 0f, 0f);

	}
}
