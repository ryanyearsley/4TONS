using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell School Data", order = 2)]
public class SpellSchoolData : ScriptableObject
{
	public int schoolIndexStart;
	public SpellSchool spellSchool;
	public WorldData worldData;
	public Color schoolGemColor;
	public string description;
	public Sprite wizardSelectIcon;
	public Sprite portrait;
	public RuntimeAnimatorController animatorController;
	public PrebuildWizardData defaultWizard;
	public PuzzleData startingStaff;
	public PuzzleData[] schoolStaffs;
	public List<SpellData> spells;
}
public enum SpellSchool {
	Light, Dark, Fire, Ice
}
