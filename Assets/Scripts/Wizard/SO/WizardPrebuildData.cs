using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Editor/PrebuildWizardData", order = 1)]
public class WizardPrebuildData : ScriptableObject
{
	public WizardSaveData wizardSaveData;
}
