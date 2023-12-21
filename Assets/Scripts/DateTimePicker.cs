using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DateTimePicker : MonoBehaviour {

	DateTime SelectedDate;
	public Text DateText;
	public Text DateFilledText;

	public void OpenDatePickerPanel()
	{
		gameObject.SetActive (true);
		Init ();
	}

	public void CloseDatePickerPanel()
	{
		gameObject.SetActive (false);
		DateFilledText.text = GetDateString ();
	}
	string [] Months;

	void Init()
	{
		Months = new string[12];
		System.DateTime iMonth = new System.DateTime(2009,1,1);
		for (int i = 0; i < 12; ++i) 
		{
			iMonth = new System.DateTime(2009, i+1, 1);
			Months[i] = iMonth.ToString("MMMM");
		}

		SelectedDate = new DateTime (1990, 1, 1);
		UpdateText ();
	}

	void UpdateText()
	{
		DateText.text = GetDateString ();
	}

 	string GetDateString()
	{
		return string.Format ("{0} : {1} : {2}", 
		SelectedDate.Day, Months [SelectedDate.Month - 1], SelectedDate.Year);
	}

	public void IncreaseDate()
	{
		SelectedDate = SelectedDate.AddDays (1);
		UpdateText ();
	}

	public void DecreaseDate()
	{
		SelectedDate = SelectedDate.AddDays (-1);
		UpdateText ();
	}

	public void IncreaseMonth()
	{
		SelectedDate = SelectedDate.AddMonths (1);
		UpdateText ();
	}

	public void DecreaseMonth()
	{
		SelectedDate = SelectedDate.AddMonths (-1);
		UpdateText ();
	}

	public void IncreaseYear()
	{
		SelectedDate = SelectedDate.AddYears (1);
		UpdateText ();
	}

	public void DecreaseYear()
	{
		SelectedDate = SelectedDate.AddYears (-1);
		UpdateText ();
	}
}
