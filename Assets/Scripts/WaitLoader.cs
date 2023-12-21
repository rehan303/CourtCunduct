using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitLoader : MonoBehaviour {
	public static WaitLoader Instance = null;

	void Awake()
	{
		if (Instance == null) {
			Instance = this;
		} else if (Instance != null) {
			Destroy (this.gameObject);
		}
	}

	public void WaitLoaderSetActive(bool status)
	{
		this.transform.GetChild (0).gameObject.SetActive (status);
	}

	// Update is called once per frame
	void Update () 
	{
		if(gameObject.activeInHierarchy)
			this.transform.GetChild (0).GetChild (0).transform.Rotate (0f, 0f, -70* Time.deltaTime);
	}
}
