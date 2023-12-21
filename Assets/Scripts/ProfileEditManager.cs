using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using SimpleJSON;
//
using System.Xml;
using System.Linq;
using System.IO;
using System ;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using UnityEngine.Networking;
using ImageVideoContactPicker;


public class ProfileEditManager : MonoBehaviour {

	public static ProfileEditManager Instance = null;
	[Header("Edit Parameters")]
	// Profile Edit Parameters Declaration
	public string UserId;
	public InputField UserName;
	public InputField EmailId;
	public Text DateOfBirth;
	public InputField Location;
	public InputField Education;
	public InputField Qualification;
	public InputField WorkPlace;
	public InputField CareerAspirations;
	public Button ProfileImage;

	[Header("Get Parameters")]
	// Profile Get Parameters Declaration
	public InputField Get_UserName;
	public InputField  Get_EmailId;
	public InputField  Get_DateOfBirth;
	public InputField  Get_Location;
	public InputField  Get_Education;
	public InputField  Get_Qualification;
	public InputField  Get_WorkPlace;
	public InputField  Get_CareerAspirations;
	public  Image Get_ProfileImage;


	public ProfileDetail getProfileDetails = new ProfileDetail();
	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != null) {
			Destroy (this.gameObject);
		}
	}
	// Use this for initialization
	void Start () {
		
	}

	public void OpenGalleryProfilePicture()
	{
		#if UNITY_ANDROID
		AndroidPicker.BrowseImage(false);
		#elif UNITY_IPHONE
		IOSPicker.BrowseImage(false); // pick
		#endif
	}

	public void ViewProfileData()
	{	

			UserId = getProfileDetails.id;
			Get_UserName.text =  getProfileDetails.UserName ;
			Get_EmailId.text =getProfileDetails.Email;
			Get_DateOfBirth.text = getProfileDetails.DoB;
			Get_Location.text = getProfileDetails.Location;
			Get_Education.text = getProfileDetails.Education;
			Get_Qualification.text = getProfileDetails.Qualification;
			Get_WorkPlace.text = getProfileDetails.Work_place;
			Get_CareerAspirations.text = getProfileDetails.CareerAspiration;
	}

	public void FillProfileData()
	{
		//Show Data on Edit Screen
		UserId = getProfileDetails.id;
		UserName.text =  getProfileDetails.UserName ;
		UserName.interactable = false;
		EmailId.text = getProfileDetails.Email;
		EmailId.interactable = false;
		DateOfBirth.text = getProfileDetails.DoB;
		Location.text = getProfileDetails.Location;
		Education.text = getProfileDetails.Education;
		Qualification.text = getProfileDetails.Qualification;
		WorkPlace.text = getProfileDetails.Work_place;
		CareerAspirations.text = getProfileDetails.CareerAspiration;
		//Show Data on Profile View Screen
		UserId = getProfileDetails.id;
		Get_UserName.text =  getProfileDetails.UserName ;
		Get_EmailId.text =getProfileDetails.Email;
		Get_DateOfBirth.text = getProfileDetails.DoB;
		Get_Location.text = getProfileDetails.Location;
		Get_Education.text = getProfileDetails.Education;
		Get_Qualification.text = getProfileDetails.Qualification;
		Get_WorkPlace.text = getProfileDetails.Work_place;
		Get_CareerAspirations.text = getProfileDetails.CareerAspiration;
	}

	public void GetUserProfileDetails ()
	{
		StartCoroutine (GetUserProfile());
	}

	IEnumerator GetUserProfile ()
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);

		UnityWebRequest request= UnityWebRequest.Get(DatabaseURL.GetUser_Profile);
		request.SetRequestHeader("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		yield return request.SendWebRequest();
		Debug.Log("DATA is - " + request.downloadHandler.text);

		if (request.error == null)
		{
			var response = JSON.Parse(request.downloadHandler.text);

			if(response["statusCode"].ToString() == "200")
			{
				string messageOnSuccess = response["status"].ToString();
				Debug.Log(messageOnSuccess);
				getProfileDetails.id = response ["data"] ["_id"].ToString ().Trim ('"');
				getProfileDetails.UserName = response ["data"] ["username"].ToString ().Trim ('"');
				getProfileDetails.Email = response ["data"] ["email"].ToString ().Trim ('"');
				getProfileDetails.RegisterType = response ["data"] ["register_type"].ToString ().Trim ('"');
				if(!response ["data"] ["education"].ToString ().Contains ("null"))
					getProfileDetails.Education = response ["data"] ["education"].ToString ().Trim ('"');

				if(!response ["data"] ["qualification"].ToString ().Contains ("null"))
					getProfileDetails.Qualification = response ["data"] ["qualification"].ToString ().Trim ('"');

				if(!response ["data"] ["dob"].ToString ().Contains ("null"))
					getProfileDetails.DoB = response ["data"] ["dob"].ToString ().Trim ('"');

				if(!response ["data"] ["work_place"].ToString ().Contains ("null"))
					getProfileDetails.Work_place = response ["data"] ["work_place"].ToString ().Trim ('"');

				if(!response ["data"] ["location"].ToString ().Contains ("null"))
					getProfileDetails.Location = response ["data"] ["location"].ToString ().Trim ('"');

				if(!response ["data"] ["career_aspiration"].ToString ().Contains ("null"))
					getProfileDetails.CareerAspiration = response ["data"] ["career_aspiration"].ToString ().Trim ('"');

				if(!response ["data"] ["case_level"].ToString ().Contains ("null"))
					getProfileDetails.Case_level = response ["data"] ["case_level"].ToString ().Trim ('"');

				if(response ["data"] ["is_tutorial"].ToString ().Contains ("true"))
					getProfileDetails.Is_tutorial = true;
				else 
					getProfileDetails.Is_tutorial = false;
				getProfileDetails.Profile_pic = response ["data"] ["profile_pic"].ToString ().Trim ('\"').Trim ('\\').TrimStart ('\"');
			//Fill Profile Data Here
				FillProfileData ();
				yield return DownloadImage (DatabaseURL.baseURL_Assets + 
				getProfileDetails.Profile_pic.Replace ("\\", " "));
				if(!BackToProfileScreenBool)
				{
					if(Registration.IsLogIn)
						ScreenManager.instance.Registration_Navigation ("OpenHomeScreen");
					else
						ScreenManager.instance.Registration_Navigation ("ProfileScreenEdit");
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
				var response = JSON.Parse (request.downloadHandler.text);
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
	IEnumerator DownloadImage(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		Debug.Log ("DATA is - " + www.text);

		if (www.error == null) {
//			var response = JSON.Parse (request.downloadHandler.text);
			var tex = www.texture;
			ProfileEditManager.Instance.ProfileImage.GetComponent<Image> ().sprite = Sprite.Create (www.texture, new Rect (0, 0, tex.width, tex.height), Vector2.zero);
			ProfileEditManager.Instance.Get_ProfileImage.sprite = Sprite.Create (www.texture, new Rect (0, 0, tex.width, tex.height), Vector2.zero);
			Debug.Log ("Image downloaded");
		}	
		else
		{
			Debug.Log (""+www.error);
		}
	}


	public void SaveProfile(bool showPopUp)
	{
		StartCoroutine (SaveAndCreateProfile(getProfileDetails.Profile_pic, Education.text, Qualification.text, DateOfBirth.text, WorkPlace.text, Location.text, CareerAspirations.text, showPopUp));
	}

	public IEnumerator SaveAndCreateProfile(string _profile, string _education, string _qualification, string _dob, string _workPlace, string _location, string _career_aspiration, bool showPopUp)
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);

		var form = new WWWForm ();
		form.AddField ("profile_pic", _profile);
		form.AddField("education", _education);
		form.AddField("qualification", _qualification);
		form.AddField ("dob", _dob);
		form.AddField ("work_place", _workPlace);
		form.AddField ("location", _location);
		form.AddField ("career_aspiration", _career_aspiration);
			
		UnityWebRequest request = UnityWebRequest.Post(DatabaseURL.User_Profile_Create, form);
		request.method = "Put";
		request.SetRequestHeader("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		yield return request.SendWebRequest();

		Debug.Log("DATA is - " + request.downloadHandler.text);
		var endTime = Time.time;

		if (request.error == null)
		{
			var response = JSON.Parse(request.downloadHandler.text);
		
			if(response["statusCode"].ToString() == "200")
			{
				string messageOnSuccess = response["status"].ToString();
				Debug.Log(messageOnSuccess);
				if(showPopUp)
					PopUpManager.instance.ShowPopUp("PROFILE UPDATE SUCCESSFULLY!!", "Your profile has beed updated successfully!!", ()=> ScreenManager.instance.Registration_Navigation("OpenHomeScreen"));
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

	public void SkipProfileScreen()
	{
		StartCoroutine (SkipUserProfileUpdate());
	}

	IEnumerator SkipUserProfileUpdate ()
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);
		var encoding = new System.Text.UTF8Encoding ();
		Dictionary<string,string> postHeader = new Dictionary<string,string> ();
		var form = new WWWForm ();

		UnityWebRequest request= UnityWebRequest.Post(DatabaseURL.Skip_ProfileUpdate, form);
		request.SetRequestHeader("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		yield return request.SendWebRequest();
		Debug.Log("DATA is - " + request.downloadHandler.text);

		if (request.error == null)
		{
			var response = JSON.Parse(request.downloadHandler.text);

			if(response["statusCode"].ToString() == "200")
			{
				string messageOnSuccess = response["status"].ToString();
				Debug.Log(messageOnSuccess);
				RegistrationWithFacebook.Instance.imgPathGallery = "";
				ScreenManager.instance.Registration_Navigation ("OpenHomeScreen");
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
				var response = JSON.Parse (request.downloadHandler.text);
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

	public void ProfileEditScreen()
	{
		ScreenManager.instance.Registration_Navigation ("ProfileScreenEdit");
	}

	public static bool BackToProfileScreenBool;
	public void BackToProfileScreen()
	{
		BackToProfileScreenBool = true;
		GetUserProfileDetails ();
//		BackToProfileScreenBool = false;
		ScreenManager.instance.Registration_Navigation ("ProfileScreen");
	}
}

[Serializable]
public class ProfileDetail
{
	public string id;
	public string UserName;
	public string Email;
	public string RegisterType;
	public string Education;
	public string Qualification;
	public string DoB;
	public string Work_place;
	public string Location;
	public string CareerAspiration;
	public string Case_level;
	public bool Is_tutorial;
	public string Profile_pic;

}

