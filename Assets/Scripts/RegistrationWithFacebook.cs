using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Facebook.MiniJSON;
using Simple_JSON;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Networking;
using System.Text.RegularExpressions;


public class RegistrationWithFacebook : MonoBehaviour {

	public static RegistrationWithFacebook Instance = null;

	public FacebookData fbData;

	public GameObject FacebookEmailPanel;

	[SerializeField]
	private InputField emailID_Facebook;
	private string emailIDText_Facebook = string.Empty;

	[SerializeField]
	public Text ErrorMessageText_Facebook;

	void Awake ()
	{	
	// this to create the instance of this class	
		if (Instance == null) {
			Instance = this;
		} else if (Instance != null) {
			Destroy (this.gameObject);
		}
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
		}

	}

	void Start()
	{
		FacebookAutoLogin ();
	}
	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp ();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log ("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}


	//Facebook Login
	public void FBLogin ()
	{
//		//Show Wait Loader
		WaitLoader.Instance.WaitLoaderSetActive (true);
		//While login with facebook please ensure this what parameter you want to access from facebook
		var perms = new List<string> () {
			"public_profile",
			"email",
			"user_friends",
			"user_birthday",
			"user_location",
			"user_hometown"
		};
		FB.LogInWithReadPermissions (perms, AuthCallback);		
		//		FB.LogInWithPublishPermissions (perms, AuthCallback);
	}


	private void AuthCallback (ILoginResult result)
	{
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log (aToken.UserId);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log (perm);
			}
			//After Login Successfully navigate to Profile Edit Screen
//			if(ScreenManager.instance.SignIn_Panel.activeInHierarchy)
//			{
//				Registration.instance.ErrorMessageText_SignIn.text = "LOGIN WITH FACEBOOK SUCCESSFULL!!";
//			}else if(ScreenManager.instance.SignUp_Panel.activeInHierarchy )
//			{
//				Registration.instance.ErrorMessageText_SignUp.text = "LOGIN WITH FACEBOOK SUCCESSFULL!!";
//			}
			StartCoroutine (GetFacebookData ());
			WaitLoader.Instance.WaitLoaderSetActive (false);
		} else {
			WaitLoader.Instance.WaitLoaderSetActive (false);
			Debug.Log ("User cancelled login");
		}
	}

	// Show profile edit screen after some wait
	IEnumerator GetFacebookData ()
	{
		yield return new  WaitForSeconds (0.5f);
		Registration.isLoggedInWithFacebook = true;
		FacebookDataQuary ();

	}

	public void FacebookDataQuary ()
	{
		FB.API ("/me?fields=id,name,email,birthday,education,hometown", HttpMethod.GET, FBQuaryAuthCallBack);
//				FB.API ("/me/scores", HttpMethod.GET, ScoresCallback);
	}


	private void FBQuaryAuthCallBack(IResult result)
	{
			JSONNode _jsnode = Simple_JSON.JSON.Parse (result.ToString ());
	print (_jsnode);
	JSONNode dataArray = _jsnode ["data"];
		RegistrationWithFacebook.Instance.fbData.id = _jsnode ["id"];
		RegistrationWithFacebook.Instance.fbData.Name = _jsnode ["name"];
		RegistrationWithFacebook.Instance.fbData.Email = _jsnode ["email"];
		RegistrationWithFacebook.Instance.fbData.Location = _jsnode ["hometown"] ["name"];
		RegistrationWithFacebook.Instance.fbData.DOB = _jsnode ["birthday"];

		//Get User Profile
		FB.API ("https" + "://graph.facebook.com/" + RegistrationWithFacebook.Instance.fbData.id.ToString () + "/picture?type=large", HttpMethod.GET, delegate (IGraphResult pictureResult) {
			if (pictureResult.Error != null) {
				Debug.Log (pictureResult.Error);
			} else {				
				ProfileEditManager.Instance.ProfileImage.GetComponent<Image> ().sprite = Sprite.Create (pictureResult.Texture, new Rect (0, 0, pictureResult.Texture.width,  pictureResult.Texture.height), new Vector2 (0, 0));
				ProfileEditManager.Instance.Get_ProfileImage.sprite =  Sprite.Create (pictureResult.Texture, new Rect (0, 0, pictureResult.Texture.width,  pictureResult.Texture.height), new Vector2 (0, 0));
			}
			if(RegistrationWithFacebook.Instance.fbData.Email == "")
			{
				StartCoroutine (FBRegistration("", false, false));
			}
			else
			{
				StartCoroutine (FBRegistration(RegistrationWithFacebook.Instance.fbData.Email, true , false)); 
			}
		});	
	}

	public void EnterEmailIDFacebook()
	{
		bool emailVerified = IsEmail(emailID_Facebook.text);
		if(emailVerified)
		{
			emailIDText_Facebook = emailID_Facebook.text; 
			ErrorMessageText_Facebook.text = string.Empty;
		}
		else
		{
			Debug.Log("Not a valid email!!");
			ErrorMessageText_Facebook.text = "Not a valid email";
			emailID_Facebook.text = string.Empty;
			emailIDText_Facebook = string.Empty;
		}
//		CheckAllFieldsNotEmptySignUp();
	}

	public static bool IsEmail(string email)
	{
		const string MatchEmailPattern =
			@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

		if (email != null)
		{
			return Regex.IsMatch(email, MatchEmailPattern);
		}
		else
		{
			return false;  
		}
	}

	public void OnClickOkFacebookEmail()
	{
		FacebookEmailPanel.SetActive (false);
		StartCoroutine (FBRegistration(emailIDText_Facebook, false , false));
	}

	public void OnClickCancelFacebookEmail()
	{
		FacebookEmailPanel.SetActive (false);
	}


	// Log out form Facebook
	public void TempLogOutLogOut ()
	{
		FB.LogOut ();
	}

	//FacebookShare
	public void FBShare ()
	{
//		FB.ShareLink (
//			new Uri ("https://www.facebook.com/globalinteractivevlog/"),
//			"Welcome to Global Intercative Vlog",
//			"This line is used for discription",
//			new Uri ("https://www.google.co.in/url?sa=i&rct=j&q=&esrc=s&source=images&cd=&cad=rja&uact=8&ved=0ahUKEwjC45vvx73XAhUKOI8KHR1pANIQjRwIBw&url=https%3A%2F%2Fmodernseoul.org%2F2013%2F02%2F28%2Ftop-5-vlogs-video-blogs-about-south-korea-korean-culture%2F&psig=AOvVaw3k8coHA9O6y49ANUHTOD3H&ust=1510731582614896"),
//			callback: ShareCallback
//		);

	}


	private void ShareCallback (IShareResult result)
	{
		if (result.Cancelled || !String.IsNullOrEmpty (result.Error)) {
			Debug.Log ("ShareLink Error: " + result.Error);
		} else if (!String.IsNullOrEmpty (result.PostId)) {
			// Print post identifier of the shared content
			Debug.Log (result.PostId);
		} else {
			// Share succeeded without postID
			Debug.Log ("ShareLink success!");
		}
	}

	public void PostScoreOnFacebook (float Score)
	{
		if (FB.IsLoggedIn) {
			var scoreData = new Dictionary<string,string> ();
			scoreData.Clear ();
			scoreData ["score"] = Score.ToString ();
			FB.API ("/me/scores", HttpMethod.POST, delegate(IGraphResult result) {
				Debug.Log ("Score submit result: " + result);
				Debug.Log ("Score is: " + Score.ToString ());
			}, scoreData);
		}
	}

	public void FacebookAutoLogin()
	{
		if(PlayerPrefs.HasKey (PrefPaths.FBSocialId))
		{
			RegistrationWithFacebook.Instance.fbData.id = PlayerPrefs.GetString (PrefPaths.FBSocialId);
			StartCoroutine (FBRegistration("", true, true));
		}		
	}

	// Registration facbook and email update
	IEnumerator FBRegistration(string email, bool status , bool isAutoLogIn)
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);
		var encoding = new System.Text.UTF8Encoding ();
		Dictionary<string,string> postHeader = new Dictionary<string,string> ();

		var json = new SimpleJSON.JSONObject();
		json.Add ("username", RegistrationWithFacebook.Instance.fbData.Name);
		json.Add("email",  email);
		json.Add("is_email", status);
		json.Add ("social_id", RegistrationWithFacebook.Instance.fbData.id);
		json.Add ("profile_pic", "");
		json.Add ("device_id", "fdsafdsa");
		json.Add ("device_type", "Ios");

		postHeader.Add ("Content-Type", "application/json");

		var bytes = encoding.GetBytes(json.ToString());
		WWW request = new WWW (DatabaseURL.User_FB_Registration,bytes, postHeader);
		var intiTime = Time.time;
		yield return request;
		Debug.Log("DATA is - " + request.text);
		var endTime = Time.time;

		var result = endTime - intiTime;
		Debug.LogError("Time is == " + result);
		if (request.error == null)
		{
			var response = JSON.Parse(request.text);

			if(response["statusCode"].ToString ().Contains ("200") )
			{
				string messageOnSuccess = response["status"].ToString();
				ProfileEditManager.BackToProfileScreenBool = false;
				var responceHeaders = request.responseHeaders ["xlogintoken"];
				print (responceHeaders);

				PlayerPrefs.SetString (PrefPaths.xToken_String, responceHeaders.ToString ());
				if (!isAutoLogIn) {
					yield return StartCoroutine (UploadImage (true));
//					ProfileEditManager.Instance.SaveProfile (false);
				}
				if (response ["data"] ["is_login"].ToString ().Contains ("true")) {
					Registration.IsLogIn = true;
				} else
					Registration.IsLogIn = false;
				
				PlayerPrefs.SetString (PrefPaths.FBSocialId, RegistrationWithFacebook.Instance.fbData.id);

				ProfileEditManager.Instance.GetUserProfileDetails ();		
							
			}else if(response["statusCode"].ToString ().Contains ("400"))
			{
				PopUpManager.instance.ShowPopUp ("PLEASE UPDATE EMAIL!!",response["message"].ToString (), ()=> FacebookEmailPanel.SetActive (true));
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
			else
			{
				Debug.Log(response["message"].ToString());
				PopUpManager.instance.ShowPopUp("ERROR!!", response["message"].ToString());
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
		}else
		{
			if(request.error == "400 Bad Request")
			{
				var response = JSON.Parse(request.text);
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

	public string imgPathGallery;
	public void UpLoadImageThenSaveProfile()
	{
		ProfileEditManager.Instance.SaveProfile (true);
	}

	public void TestUp()
	{
		StartCoroutine (UploadImage( false ,""));
	}
	// Uploade Image To server 
	public IEnumerator UploadImage (bool isFromFb, string path = null)
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);


		WWW localFile = null;
		byte[] bytesdata = new byte[0];
		Texture2D texture = null;

		if (!isFromFb) {
			#if UNITY_EDITOR
			path = "/Users/igniva-gaming-03/Desktop/IMG_4289.jpg";
			#endif
		if (File.Exists (path)) {
			localFile = new WWW ("file://" + path);
			yield return localFile;
//				var uncompressed = localFile.texture;
//				uncompressed.Compress (false);
			bytesdata = localFile.texture.EncodeToJPG (10);
			texture = localFile.texture;
		}
		else{
				WaitLoader.Instance.WaitLoaderSetActive (false);
				yield break;
			}
		} else {
			var textureImg = ProfileEditManager.Instance.ProfileImage.GetComponent<Image> ().sprite.texture;
			bytesdata = textureImg.EncodeToPNG ();
			texture = textureImg;
		}			

		UnityEngine.WWWForm form = new UnityEngine.WWWForm ();
		form.AddBinaryData ("image", bytesdata);

		UnityWebRequest request = UnityWebRequest.Post (DatabaseURL.UploadProfileImage, form);
		request.SetRequestHeader ("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		request.chunkedTransfer = false;
		yield return request.SendWebRequest ();
		Debug.Log ("DATA is - " + request.downloadHandler.text);
		var endTime = Time.time;

		if (request.error == null) {
			var response = JSON.Parse (request.downloadHandler.text);

			if (request.responseCode == 200) {
				string messageOnSuccess = response ["status"].ToString ();
				ProfileEditManager.Instance.getProfileDetails.Profile_pic = response ["data"].ToString ().Trim ('\\');
			ProfileEditManager.Instance.ProfileImage.GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0,texture.width,texture.height), Vector2.zero);
				Debug.Log (messageOnSuccess);
				WaitLoader.Instance.WaitLoaderSetActive (false);
			} else {
				Debug.Log (response ["message"].ToString ());
				PopUpManager.instance.ShowPopUp ("ERROR!!", response ["message"].ToString ());
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
		} else {
			if (request.error == "400 Bad Request") {
				var response = JSON.Parse (request.downloadHandler.text);
				string message = response ["message"].ToString ();
				Debug.Log (message);
				PopUpManager.instance.ShowPopUp ("ERROR!!", message);
				WaitLoader.Instance.WaitLoaderSetActive (false);
			} else {
				Debug.Log ("ERROR OCCURED!!");
				PopUpManager.instance.ShowPopUp ("ERROR OCCURED!!", "Please try again Later");
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
		}
	}
}


[Serializable]
public class FacebookData
{
	public string id;
	public string Name;
	public string Email;
	public string DOB;
		public string Location;

	}