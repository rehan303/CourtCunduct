using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PopUpManager : MonoBehaviour {

    public static PopUpManager instance;

    [SerializeField]
    private GameObject PopUp;

    [SerializeField]
    private Text PopUp_Title;

    [SerializeField]
    private Text PopUp_Description;

    [SerializeField]
    private Button confirmationButton;

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

    public void ShowPopUp(string _title, string _description, UnityAction onClickOK = null)
    {
        PopUp.SetActive(true);
        PopUp_Title.text = _title;
        PopUp_Description.text = _description;
        confirmationButton.onClick.RemoveAllListeners();
        if(onClickOK != null)
        {
            confirmationButton.onClick.AddListener(onClickOK);
        }
        confirmationButton.onClick.AddListener(()=>ClosePopUp());
       
    }

    public void ClosePopUp()
    {
        PopUp.SetActive(false);
    }
}
