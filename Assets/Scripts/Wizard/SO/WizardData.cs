using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Editor/WizardData", order = 1)]

public class WizardData : CreatureData {

	public SpellSchool school;
	public SpellSchoolData schoolData;
	public Sprite wizardSelectIcon;
	public Sprite portrait;
	public RuntimeAnimatorController animatorController;
	public WizardPrebuildData wizardBuildData;
}

