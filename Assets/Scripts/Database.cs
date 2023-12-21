using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    
}

public static class DatabaseURL
{
	public const string Registration_SignUp = "http://192.168.0.239:6015/v1/user/registration";//"https://courtconductapi.ignivastaging.com/v1/user/registration";
	public const string Registration_SignIn = "http://192.168.0.239:6015/v1/user/login";
	public const string Registration_ForgotPassword = "http://192.168.0.239:6015/v1/user/forgot-password";
	public const string User_Profile_Create ="http://192.168.0.239:6015/v1/user/create-profile";
	public const string User_FB_Registration = "http://192.168.0.239:6015/v1/user/fb-registration";
	public const string UploadProfileImage = "http://192.168.0.239:6015/v1/user/upload-images";
	public const string GetUser_Profile = "http://192.168.0.239:6015/v1/user/get-profile";
	public const string baseURL_Assets = "http://192.168.0.239:6015/Assets/";
	public const string Skip_ProfileUpdate = "http://192.168.0.239:6015/v1/user/skip-profile";
	public const string GetSaveStateOfTheGame = "http://192.168.0.239:6015/v1/score/get-score";
	public const string PostSaveStateOfTheGame = "http://192.168.0.239:6015/v1/score/save-score";
	public const string LogoutFromTheGame = "http://192.168.0.239:6015/v1/user/logout";
}

public static class PrefPaths
{
	public const string xToken_String = "xToken";
	public const string FBSocialId = "FBSocialId";

	public const string Score = "Scores";
	public const string Case1Level1_Scores = "Case1Level1Scores";
	public const string Case1Level2_Scores = "Case1Level2Scores";
	public const string Case1Level3_Scores = "Case1Level3Scores";
	public const string Case1Level4_Scores = "Case1Level4Scores";
	public const string Case1Level5_Scores = "Case1Level5Scores";

	public const string RegistrationSceneBackFromGamePlay = "NO";
	public const string EmailId = "AutoLoginEmailId";
	public const string password = "AutoLoginPassword";
}