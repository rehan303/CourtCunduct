using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour 
{

	[SerializeField]
	private GameObject[] UnSelectedCharacters;
	[SerializeField]
	private GameObject[] SelectedCharacters;
	[SerializeField]
	private GameObject[] CharacterSelectedButtons;

	private int currentCharacterSelected = 0;
	private int previousCharacterSelected = 0;

	public void LeftArrowButtonClicked()
	{
		if(currentCharacterSelected > 0)
		{
			previousCharacterSelected = currentCharacterSelected;
			currentCharacterSelected -= 1;
		}
		SelectedCharacters [previousCharacterSelected].SetActive (false);
		UnSelectedCharacters [previousCharacterSelected].SetActive (true);
		UnSelectedCharacters [currentCharacterSelected].SetActive (false);
		SelectedCharacters [currentCharacterSelected].SetActive (true);
		CharacterSelectedButtons [previousCharacterSelected].SetActive (false);
		CharacterSelectedButtons [currentCharacterSelected].SetActive (true);
	}

	public void RightArrowButtonClicked()
	{
		if(currentCharacterSelected < 3)
		{
			previousCharacterSelected = currentCharacterSelected;
			currentCharacterSelected += 1;
		}
		SelectedCharacters [previousCharacterSelected].SetActive (false); 
		UnSelectedCharacters [previousCharacterSelected].SetActive (true);
		UnSelectedCharacters [currentCharacterSelected].SetActive (false);
		SelectedCharacters [currentCharacterSelected].SetActive (true);
		CharacterSelectedButtons [previousCharacterSelected].SetActive (false);
		CharacterSelectedButtons [currentCharacterSelected].SetActive (true);
	}
}
