using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyDisplayNameButtonClick : AbstractButtonClick {
	private PlayfabSettingsUI settingsScreenUI;
	private void Start () {
		settingsScreenUI = GetComponentInParent<PlayfabSettingsUI> ();
	}
	public override void OnClick () {
		base.OnClick ();
		settingsScreenUI.ApplySettingsUpdate ();
	}
}
