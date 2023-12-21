using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class Registration : MonoBehaviour 
{

    public static Registration instance;


    // Sign Up Parameters Declaration
    [SerializeField]
	private InputField username_SignUp;
    private string usernameText_SignUp = string.Empty;

    [SerializeField]
	private InputField emailID_SignUp;
    private string emailIDText_SignUp = string.Empty;

    [SerializeField]
	private InputField password_SignUp;
    private string passwordText_SignUp = string.Empty;

    [SerializeField]
	private InputField confirmPassword_SignUp;
    private string confirmPasswordText_SignUp = string.Empty;

    [SerializeField]
	private InputField emailID_SignIn;
    private string emailIDText_SignIn = string.Empty;

    [SerializeField]
	private InputField password_SignIn;
    private string passwordText_SignIn = string.Empty;

    [SerializeField]
	private InputField emailID_ForgotPassword;
    private string emailIDText_ForgotPassword = string.Empty;

    [SerializeField]
    private Button Registration_SignUpButton;

    [SerializeField]
    private Button Registration_LoginButton;

    [SerializeField]
    private Button Registration_ForgotPasswordButton;

    [SerializeField]
	public Text ErrorMessageText_SignUp;

    [SerializeField]
	public Text ErrorMessageText_SignIn;

    [SerializeField]
	public Text ErrorMessageText_ForgotPassword;

	public static bool isLoggedInWithFacebook;
	public static bool IsLogIn;

	// Use this for initialization
	void Awake () 
    {
        if(instance == null)
        {
            instance = this;
		}


	}

	void Start ()
	{
//		StartCoroutine(SignIn_Registration(PlayerPrefs.GetString (PrefPaths.EmailId),PlayerPrefs.GetString (PrefPaths.password)));
	}
	
	// Update is called once per frame
	void Update () 
    {
	    	
	}

    // SIGN UP REGISTRATION
    // Input User Name in Sign Up
    public void EnterUserNameSignUp()
    {
        if(username_SignUp.text.Length < 5)
        {
			username_SignUp.text = string.Empty;
			usernameText_SignUp = string.Empty;
            Debug.Log("Username is too Short. Please enter again!!");
            ErrorMessageText_SignUp.text = "Username should be 5-12 characters";
        }
        else if(username_SignUp.text.Length > 12)
        {
			username_SignUp.text = string.Empty;
			usernameText_SignUp = string.Empty;
            Debug.Log("Username is too Long. Please enter again!!");
            ErrorMessageText_SignUp.text = "Username should be 5-12 characters";
        }
        else
        {
            usernameText_SignUp = username_SignUp.text;
			ErrorMessageText_SignUp.text = string.Empty;
        }
        CheckAllFieldsNotEmptySignUp();
    }

    // Input Email ID in Sign Up
    public void EnterEmailIDSignUp()
    {
        bool emailVerified = IsEmail(emailID_SignUp.text);
        if(emailVerified)
        {
            emailIDText_SignUp = emailID_SignUp.text; 
			ErrorMessageText_SignUp.text = string.Empty;
        }
        else
        {
            Debug.Log("Not a valid email!!");
            ErrorMessageText_SignUp.text = "Not a valid email";
			emailID_SignUp.text = string.Empty;
			emailIDText_SignUp = string.Empty;
        }
        CheckAllFieldsNotEmptySignUp();
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

    // Input Password in Sign Up
    public void EnterPasswordSignUp()
    {
        if(password_SignUp.text.Length < 6)
        {
			password_SignUp.text = string.Empty;
			passwordText_SignUp = string.Empty;
            Debug.Log("Password is too Short. Please enter again!!");
            ErrorMessageText_SignUp.text = "Password should be alphanumeric";
        }
        else if(password_SignUp.text.Length > 20)
        {
			password_SignUp.text = string.Empty;
			passwordText_SignUp = string.Empty;
            Debug.Log("Password is too Long. Please enter again!!");
            ErrorMessageText_SignUp.text = "Password should be alphanumeric";
        }
        else
        {
            passwordText_SignUp = password_SignUp.text;
			ErrorMessageText_SignUp.text = string.Empty;
        }
        CheckAllFieldsNotEmptySignUp();
    }

    // Input Confirm Password in Sign up
    public void EnterConfirmPasswordSignUp()
    {
        if(passwordText_SignUp == confirmPassword_SignUp.text)
        {
            confirmPasswordText_SignUp = confirmPassword_SignUp.text;
			ErrorMessageText_SignUp.text = string.Empty;
        }
        else
        {
            Debug.Log("Password does not Match!!");
			confirmPassword_SignUp.text = string.Empty;
			confirmPasswordText_SignUp = string.Empty;
            ErrorMessageText_SignUp.text = "Password does not Match!!";
        }
        CheckAllFieldsNotEmptySignUp();
    }

    public void CheckAllFieldsNotEmptySignUp()
    {
        if(usernameText_SignUp != string.Empty && emailIDText_SignUp != string.Empty && passwordText_SignUp != string.Empty && confirmPasswordText_SignUp != string.Empty)
        {
            Registration_SignUpButton.interactable = true;
        }
        else
        {
            Registration_SignUpButton.interactable = false; 
        }
    }

    public void ClearAllFieldsSignUp()
    {
        usernameText_SignUp = string.Empty;
        emailIDText_SignUp = string.Empty;
        passwordText_SignUp = string.Empty;
        confirmPasswordText_SignUp = string.Empty;

        username_SignUp.text = string.Empty;
        emailID_SignUp.text = string.Empty;
        password_SignUp.text = string.Empty;
        confirmPassword_SignUp.text = string.Empty;

        ErrorMessageText_SignUp.text = string.Empty;

    }

    public void Registration_OnClickSignUp()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
			Registration_SignUpButton.interactable = false; 
            StartCoroutine(SignUp_Registration(usernameText_SignUp, emailIDText_SignUp, passwordText_SignUp));
        }
        else
        {
            PopUpManager.instance.ShowPopUp("ERROR OCCURED!!", "Internet not working");
        }
    }

    IEnumerator SignUp_Registration(string _username, string _email, string _password)
    {
		WaitLoader.Instance.WaitLoaderSetActive (true);
        var encoding = new System.Text.UTF8Encoding ();
        Dictionary<string,string> postHeader = new Dictionary<string,string> ();

        var json = new SimpleJSON.JSONObject();
        json.Add("username", _username);
        json.Add("email", _email);
        json.Add("password", _password);

        postHeader.Add ("Content-Type", "application/json");

        var bytes = encoding.GetBytes(json.ToString());
        WWW request = new WWW (DatabaseURL.Registration_SignUp,bytes, postHeader);
        var intiTime = Time.time;
        yield return request;
        Debug.Log("DATA is - " + request.text);
        var endTime = Time.time;

        var result = endTime - intiTime;
        Debug.LogError("Time is == " + result);
        if (request.error == null)
        {
            var response = JSON.Parse(request.text);
//            {
//                "statusCode": 200,
//                "status": "success",
//                "message": "Please verify the link sent to your email & proceed with login."
//            }
            if(response["statusCode"].ToString() == "200")
            {
                string messageOnSuccess = response["status"].ToString();
                Debug.Log(messageOnSuccess);
                PopUpManager.instance.ShowPopUp("SIGNUP SUCCESSFULL!!", "Mail has been sent to User. Please verify!!", ()=> ScreenManager.instance.Registration_Navigation("Login"));
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

    public void EnterEmailIDSignIn()
    {
        bool emailVerified = IsEmail(emailID_SignIn.text);
        if(emailVerified)
        {
            emailIDText_SignIn = emailID_SignIn.text;
			ErrorMessageText_SignIn.text = string.Empty;
        }
        else
        {
            Debug.Log("Not a valid email!!");
            ErrorMessageText_SignIn.text = "Not a valid email!!";
			emailID_SignIn.text = string.Empty;
			emailIDText_SignIn = string.Empty;
        }
        CheckAllFieldsNotEmptySignIn();
    }

    public void EnterPasswordSignIn()
    {
        if(password_SignIn.text.Length < 6)
        {
			password_SignIn.text = string.Empty;
			passwordText_SignIn = string.Empty;
            Debug.Log("Password is too Short. Please enter again!!");
            ErrorMessageText_SignIn.text = "Not a valid password";
        }
        else if(password_SignUp.text.Length > 20)
        {
			password_SignIn.text = string.Empty;
			passwordText_SignIn = string.Empty;
            Debug.Log("Password is too Long. Please enter again!!");
            ErrorMessageText_SignIn.text = "Not a valid password";
        }
        else
        {
            passwordText_SignIn = password_SignIn.text;
			ErrorMessageText_SignIn.text = string.Empty;
        }
        CheckAllFieldsNotEmptySignIn();
    }

    public void CheckAllFieldsNotEmptySignIn()
    {
        if(emailIDText_SignIn != string.Empty && passwordText_SignIn != string.Empty)
        {
            Registration_LoginButton.interactable = true;
        }
        else
        {
            Registration_LoginButton.interactable = false;
        }
    }

    public void ClearAllFieldsSignIn()
    {
        emailIDText_SignIn = string.Empty;
        passwordText_SignIn = string.Empty;

        emailID_SignIn.text = string.Empty;
        password_SignIn.text = string.Empty;

        ErrorMessageText_SignIn.text = string.Empty;
    }

    public void Registration_OnClickLogin()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
			Registration_LoginButton.interactable = false;
//			if(emailIDText_SignIn == "" && passwordText_SignIn == "")
//				StartCoroutine(SignIn_Registration("aditi.goel@ignivasolutions.com", "aditi1391"));
//			else
				StartCoroutine(SignIn_Registration(emailIDText_SignIn, passwordText_SignIn));
        }
        else
        {
            PopUpManager.instance.ShowPopUp("ERROR OCCURED!!", "Internet not working");
        }
    }
    public IEnumerator SignIn_Registration(string _email, string _password)
    {

		WaitLoader.Instance.WaitLoaderSetActive (true);
        var encoding = new System.Text.UTF8Encoding ();
        Dictionary<string,string> postHeader = new Dictionary<string,string> ();

        var json = new SimpleJSON.JSONObject();
        json.Add("email", _email);
        json.Add("password", _password);
        json.Add("device_id", "12345");
        json.Add("device_type", "Ios");

        postHeader.Add ("Content-Type", "application/json");

        var bytes = encoding.GetBytes(json.ToString());
        WWW request = new WWW (DatabaseURL.Registration_SignIn,bytes, postHeader);
        yield return request;
        Debug.Log("DATA is - " + request.text);

        if (request.error == null)
        {
            var response = JSON.Parse(request.text);
//            {
//                "statusCode": 200,
//                "status": "success",
//                "message": "Logged in successfully",
//                "data": {
//                    "id": "5aeb01a6e02600b1440ec78d",
//                    "username": "madhur01",
//                    "email": "madhur@yopmail.com",
//                    "profile_pic": "1525351165_1634264.png",
//                    "baseUrl": "http://courtconductapi.ignivastaging.com:6015/Assets/",
//                    "register_type": "Email"
//                }
//            }
            if(response["statusCode"].ToString() == "200")
            {
                Debug.Log("User Login Successfully");
				Registration.isLoggedInWithFacebook = false;

				//Save responce data if needed
				if (response ["data"] ["is_login"].ToString ().Contains ("true")) 
				{
					IsLogIn = true;
				} 
				else
				{
					IsLogIn = false;
				}
				PlayerPrefs.SetString (PrefPaths.EmailId, _email);
				PlayerPrefs.SetString (PrefPaths.password, _password);
				ProfileEditManager.BackToProfileScreenBool = false;
				var responceHeaders = request.responseHeaders ["xlogintoken"];
				print (responceHeaders);
				PlayerPrefs.SetString (PrefPaths.xToken_String, responceHeaders.ToString ());
				//Get User Profile Details
				if (!PlayerPrefs.HasKey (PrefPaths.EmailId) && !PlayerPrefs.HasKey (PrefPaths.password))
					PopUpManager.instance.ShowPopUp ("LOGIN SUCCESSFULL!!", "USER IS SUCCESSFULLY LOGIN!!", () => ProfileEditManager.Instance.GetUserProfileDetails ());
				else
					ProfileEditManager.Instance.GetUserProfileDetails ();
				WaitLoader.Instance.WaitLoaderSetActive (false);
            }
            else
            {
                Debug.Log(response["message"].ToString());
                PopUpManager.instance.ShowPopUp("ERROR OCCURED!!", response["message"].ToString());
				WaitLoader.Instance.WaitLoaderSetActive (false);
            }
        }
        else
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

    // FORGOT PASSWORD REGISTRATION
    // FORGOT PASSWORD EMAIL INPUT
    public void EnterEmailIDForgotPassword()
    {
        bool emailVerified = IsEmail(emailID_ForgotPassword.text);
        if(emailVerified)
        {
            emailIDText_ForgotPassword = emailID_ForgotPassword.text;
			ErrorMessageText_ForgotPassword.text = string.Empty;
            Registration_ForgotPasswordButton.interactable = true;
        }
        else
        {
            Debug.Log("Not a valid email!!");
            ErrorMessageText_ForgotPassword.text = "Not a valid email!!";
			emailID_ForgotPassword.text = string.Empty;
			emailIDText_ForgotPassword = string.Empty;
        }
    }

    public void ClearAllFieldsForgotPassword()
    {
        emailIDText_ForgotPassword = string.Empty;

        emailID_ForgotPassword.text = string.Empty;

        ErrorMessageText_ForgotPassword.text = string.Empty;
    }

    public void Registration_OnClickForgotPassword()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
			Registration_ForgotPasswordButton.interactable = false;
            StartCoroutine(ForgotPassword_Registration(emailIDText_ForgotPassword));
        }
        else
        {
            PopUpManager.instance.ShowPopUp("ERROR OCCURED!!", "Internet not working");
        }
    }

    IEnumerator ForgotPassword_Registration(string _email)
    {
		WaitLoader.Instance.WaitLoaderSetActive (true);
        var encoding = new System.Text.UTF8Encoding ();
        Dictionary<string,string> postHeader = new Dictionary<string,string> ();

        var json = new SimpleJSON.JSONObject();
        json.Add("email", _email);

        postHeader.Add ("Content-Type", "application/json");

        var bytes = encoding.GetBytes(json.ToString());
        WWW request = new WWW (DatabaseURL.Registration_ForgotPassword,bytes, postHeader);
        yield return request;
        Debug.Log("DATA is - " + request.text);

        if (request.error == null)
        {
            var response = JSON.Parse(request.text);
//            {
//                "statusCode": 200,
//                "status": "success",
//                "message": "A link to reset password has been sent to your registered email.",
//                "data": ""
//            }
            if(response["statusCode"].ToString() == "200")
            {
                Debug.Log("A Link to reset password has been sent");
                string messageOnSuccess = response["message"].ToString();
                PopUpManager.instance.ShowPopUp("MAIL SENT!!", messageOnSuccess, ()=> ScreenManager.instance.Registration_Navigation("CrossForgot"));
				WaitLoader.Instance.WaitLoaderSetActive (false);
            }
            else
            {
                Debug.Log(response["message"].ToString());
                PopUpManager.instance.ShowPopUp("ERROR OCCURED!!", response["message"].ToString());
				WaitLoader.Instance.WaitLoaderSetActive (false);
            }
        }
        else
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

	public void UserLogoutFromTheGame()
	{
		StartCoroutine (LogoutFromTheGame ());
	}

	public IEnumerator LogoutFromTheGame()
	{
		WaitLoader.Instance.WaitLoaderSetActive (true);
		UnityWebRequest request= UnityWebRequest.Post(DatabaseURL.LogoutFromTheGame,"");
		request.SetRequestHeader("xlogintoken", PlayerPrefs.GetString (PrefPaths.xToken_String));
		yield return request.SendWebRequest();

//		{
//			"statusCode": 200,
//			"status": "success",
//			"message": "User logged out successfully.",
//			"data": ""
//		}

		if (request.error == null) 
		{
			var response = JSON.Parse (request.downloadHandler.text);
			if (response ["statusCode"].ToString () == "200") 
			{
				PopUpManager.instance.ShowPopUp ("Logout Successfully!!", "User is Logout Successfully", () => ScreenManager.instance.Registration_Navigation ("HomeScreenBack"));
//				HomeScreenManager.instance.ResetPositionForAnimations ();
			}
			else
			{
				Debug.Log(response["message"].ToString());
				PopUpManager.instance.ShowPopUp("ERROR!!", response["message"].ToString());
				WaitLoader.Instance.WaitLoaderSetActive (false);
			}
			WaitLoader.Instance.WaitLoaderSetActive (false);
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
}
