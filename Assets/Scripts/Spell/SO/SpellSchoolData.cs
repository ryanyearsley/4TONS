using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell School Data", order = 2)]
public class SpellSchoolData : ScriptableObject
{
	public SpellSchool spellSchool;
	public Color schoolGemColor;
	public string description;
	public Sprite wizardSelectIcon;
	public Sprite portrait;
	public RuntimeAnimatorController animatorController;
	public StaffData staff;
	public List<SpellData> spells;
}
public enum SpellSchool {
	Light, Dark, Fire, Ice
}
