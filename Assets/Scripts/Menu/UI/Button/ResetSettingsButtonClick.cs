using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSettingsButtonClick : AbstractButtonClick {

	SettingsScreenUI settingsScreenUI;

	protected override void Awake () {
		base.Awake ();
		settingsScreenUI = GetComponentInParent<SettingsScreenUI> ();
	}
	public override void OnClick () {
		base.OnClick (); 
		SettingsManager.instance.ResetSettingsData ();

	}
}
