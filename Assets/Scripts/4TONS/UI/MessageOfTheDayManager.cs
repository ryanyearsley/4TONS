using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageOfTheDayManager : MonoBehaviour
{
	#region Singleton
	public static MessageOfTheDayManager instance;
	void SingletonInitialization () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}
	#endregion

	private void Awake () {
		SingletonInitialization ();
	}

	[SerializeField]
	private TextMeshProUGUI messageText;

	public void UpdateMessageOfTheDay(string message) {
		messageText.text = message;
	}
}
