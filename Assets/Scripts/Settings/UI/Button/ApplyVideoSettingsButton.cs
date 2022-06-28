using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyVideoSettingsButton : AbstractButtonClick
{
	private VideoSettingsUI videoSettingsUI;
	private void Start () {
		videoSettingsUI = GetComponentInParent<VideoSettingsUI> ();
	}
	public override void OnClick () {
		base.OnClick ();
		videoSettingsUI.ApplySettingsUpdate ();
	}
}