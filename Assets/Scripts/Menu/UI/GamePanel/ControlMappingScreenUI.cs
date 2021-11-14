using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired.UI.ControlMapper;

public class ControlMappingScreenUI : AbstractScreenUI
{
	public ControlMapper controlMapper;

	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		if (screenActiveStates.Contains (mainMenuScreen)) {
			screenObject.SetActive (true);
			controlMapper.Open ();
		} else {
			controlMapper.Close (true);
			screenObject.SetActive (false);
		}
	}

}
