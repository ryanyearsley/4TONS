using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeWizardButtonClick : AbstractButtonClick
{
	private WizardCreatePanelUI wizardCreatePanelUI;
	protected override void Awake () {
		base.Awake ();
		wizardCreatePanelUI = GetComponentInParent<WizardCreatePanelUI> ();
	}

	protected override void OnClick () {
		AudioManager.instance.PlaySound ("Randomize");
		wizardCreatePanelUI.SelectRandomName ();
	}
}
