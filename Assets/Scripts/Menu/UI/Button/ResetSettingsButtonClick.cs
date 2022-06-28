using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetSettingsButtonClick : AbstractButtonClick {


	public override void OnClick () {
		base.OnClick (); 
		SettingsManager.instance.ResetAudioSettings ();
	}
}
