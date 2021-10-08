using UnityEngine;
[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/WizardData", order = 1)]

public class WizardData : ScriptableObject {
	public SpellSchoolData schoolData;
	public Sprite wizardSelectIcon;
	public Sprite portrait;
	public Sprite spectralPreview;//used in certain spells as a projection of the wizard/player.
	public RuntimeAnimatorController animatorController;
	public WizardPrebuildData gauntletStartData;
}

