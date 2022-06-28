
using UnityEngine;

public class HigherFramerateButton : AbstractButtonClick {

	private VideoSettingsUI videoSettingsUI;
	private void Start () {
		videoSettingsUI = GetComponentInParent<VideoSettingsUI> ();
	}
	public override void OnClick () {
		base.OnClick ();
		videoSettingsUI.HigherFramerateButtonPressed ();
	}
}
