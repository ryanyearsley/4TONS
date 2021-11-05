using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplySettingsButtonClick : AbstractButtonClick
{
	private SettingsScreenUI settingsScreenUI;
	private void Start () {
		settingsScreenUI = GetComponentInParent<SettingsScreenUI> ();
	}
	public override void OnClick () {
		base.OnClick ();
		settingsScreenUI.ApplySettings ();
	}
}
