using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetVideoSettingsButton : AbstractButtonClick {
	public override void OnClick () {
		base.OnClick ();
		SettingsManager.instance.ResetVideoSettings ();
	}
}
