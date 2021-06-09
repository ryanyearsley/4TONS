using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Data", menuName = "ScriptableObjects/Wizardry/Spell School Data", order = 2)]
public class SpellSchoolData : ScriptableObject
{
	public int schoolIndexStart;
	public SpellSchool spellSchool;
	public string schoolDescription;
	public WizardData wizardData;//creature data +
	public WizardPrebuildData wizardPrebuildData;
	public Color schoolColor;
	public PuzzleData[] staffs;
	public List<SpellData> spells;
}

