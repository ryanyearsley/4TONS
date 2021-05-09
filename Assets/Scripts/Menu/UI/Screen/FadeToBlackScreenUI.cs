using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeToBlackScreenUI : AbstractScreenUI
{
	[SerializeField]
	private Animator fadeToBlackAnimator;

	protected override void OnScreenChange (MenuScreen mainMenuScreen) {
		base.OnScreenChange (mainMenuScreen);
		if (mainMenuScreen == MenuScreen.FADE_TO_BLACK) {
			fadeToBlackAnimator.SetTrigger ("FadeToBlack");
		}
		else if (mainMenuScreen == MenuScreen.WELCOME) {
			fadeToBlackAnimator.SetTrigger ("FadeOpen");
		}
	}
}
