using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDisplayNameButtonClick : AbstractButtonClick {
	private SettingsScreenUI settingsScreenUI;
	private void Start () {
		settingsScreenUI = GetComponentInParent<SettingsScreenUI> ();
	}
	public override void OnClick () {
		base.OnClick ();
		settingsScreenUI.UpdateDisplayName ();
	}
}
