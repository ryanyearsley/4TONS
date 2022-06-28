using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using System;
using PlayFab;

public class SettingsScreenUI : AbstractScreenUI
{

	

	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
			//Load settings fresh?
		} else {
			if (screenObject.activeInHierarchy == true){
				//SettingsManager.instance.RevertSettingsData ();

				screenObject.SetActive (false);
			}

		}
	}

	//called any time a settings UI element is changed
	


}
