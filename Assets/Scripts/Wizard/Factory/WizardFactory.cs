using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Input: WizardSaveData
//Instantiates wizard prefab and injects save data into game object.
public class WizardFactory 
{
	void CreateWizard(WizardSaveData wizardSaveData) {
		GameObject go = GameObject.Instantiate (ConstantsManager.instance.playerWizardPrefab);

	}
}
