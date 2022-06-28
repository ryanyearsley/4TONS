using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyAudioSettingsButtonClick : AbstractButtonClick
{
	private AudioSettingsUI audioSettingsUI;
	private void Start () {
		audioSettingsUI = GetComponentInParent<AudioSettingsUI> ();
	}
	public override void OnClick () {
		base.OnClick ();
		audioSettingsUI.ApplySettingsUpdate ();
	}
}
